using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Authentication.Commands;
using FindTheBug.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace FindTheBug.Application.Features.Authentication.Handlers;

public class RevokeTokenCommandHandler(
    IUnitOfWork unitOfWork,
    IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<RevokeTokenCommand, bool>
{
    public async Task<ErrorOr<bool>> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        // Find refresh token
        var refreshTokens = await unitOfWork.Repository<RefreshToken>().GetAllAsync(cancellationToken);
        var refreshToken = refreshTokens.FirstOrDefault(rt => rt.Token == request.RefreshToken);

        if (refreshToken is null)
            return Error.NotFound("Authentication.TokenNotFound", "Refresh token not found");

        if (refreshToken.IsRevoked)
            return true; // Already revoked

        // Revoke token
        refreshToken.RevokedAt = DateTime.UtcNow;
        refreshToken.RevokedByIp = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        refreshToken.ReasonRevoked = "Revoked by user";
        await unitOfWork.Repository<RefreshToken>().UpdateAsync(refreshToken, cancellationToken);

        return true;
    }
}
