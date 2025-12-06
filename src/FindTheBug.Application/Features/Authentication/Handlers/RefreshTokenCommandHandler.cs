using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Authentication.Commands;
using FindTheBug.Application.Features.Authentication.Contracts;
using FindTheBug.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Authentication.Handlers;

public class RefreshTokenCommandHandler(
    IUnitOfWork unitOfWork,
    IAuthenticationService authService,
    IHttpContextAccessor httpContextAccessor) 
    : ICommandHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    public async Task<ErrorOr<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // Find refresh token
        var refreshTokens = await unitOfWork.Repository<RefreshToken>().GetAllAsync(cancellationToken);
        var refreshToken = refreshTokens.FirstOrDefault(rt => rt.Token == request.RefreshToken);

        if (refreshToken is null)
            return Error.Unauthorized("Authentication.InvalidToken", "Invalid refresh token");

        // Check if token is active
        if (!refreshToken.IsActive)
        {
            if (refreshToken.IsRevoked)
                return Error.Unauthorized("Authentication.TokenRevoked", "Refresh token has been revoked");
            
            return Error.Unauthorized("Authentication.TokenExpired", "Refresh token has expired");
        }

        // Get user
        var user = await unitOfWork.Repository<User>().GetByIdAsync(refreshToken.UserId, cancellationToken);
        if (user is null)
            return Error.NotFound("User.NotFound", "User not found");

        if (!user.IsActive)
            return Error.Unauthorized("Authentication.AccountDisabled", "Account is disabled");

        // Get user roles
        var userRoles = await unitOfWork.Repository<UserRole>()
            .GetQueryable()
            .Where(ur => ur.UserId == user.Id)
            .Select(ur => ur.Role.Name)
            .ToListAsync(cancellationToken);
        var rolesString = string.Join(",", userRoles);

        // Generate new tokens
        var newAccessToken = authService.GenerateAccessToken(user.Id, user.Email ?? user.Phone, rolesString);
        var newRefreshToken = authService.GenerateRefreshToken();

        // Revoke old refresh token and create new one
        refreshToken.RevokedAt = DateTime.UtcNow;
        refreshToken.RevokedByIp = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        refreshToken.ReplacedByToken = newRefreshToken;
        refreshToken.ReasonRevoked = "Replaced by new token";
        await unitOfWork.Repository<RefreshToken>().UpdateAsync(refreshToken, cancellationToken);

        // Save new refresh token
        var newRefreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedByIp = refreshToken.RevokedByIp ?? string.Empty
        };
        await unitOfWork.Repository<RefreshToken>().AddAsync(newRefreshTokenEntity, cancellationToken);

        return new RefreshTokenResponse(
            newAccessToken,
            newRefreshToken,
            newRefreshTokenEntity.ExpiresAt
        );
    }
}
