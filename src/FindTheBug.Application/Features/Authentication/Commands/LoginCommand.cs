using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace FindTheBug.Application.Features.Authentication.Commands;

public record LoginCommand(
    string Email,
    string Password
) : ICommand<LoginResponse>;

public record LoginResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    UserInfo User
);

public record UserInfo(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string Roles
);

public class LoginCommandHandler(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IAuthenticationService authService,
    IHttpContextAccessor httpContextAccessor) 
    : ICommandHandler<LoginCommand, LoginResponse>
{
    public async Task<ErrorOr<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Get user by email
        var users = await unitOfWork.Repository<User>().GetAllAsync(cancellationToken);
        var user = users.FirstOrDefault(u => u.Email.ToLower() == request.Email.ToLower());

        if (user is null)
            return Error.Unauthorized("Authentication.InvalidCredentials", "Invalid email or password");

        // Check if account is locked
        if (user.LockedOutUntil.HasValue && user.LockedOutUntil > DateTime.UtcNow)
            return Error.Unauthorized("Authentication.AccountLocked", "Account is temporarily locked due to multiple failed login attempts");

        // Verify password
        if (!passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            // Increment failed login attempts
            user.FailedLoginAttempts++;
            if (user.FailedLoginAttempts >= 5)
            {
                user.LockedOutUntil = DateTime.UtcNow.AddMinutes(15);
            }
            await unitOfWork.Repository<User>().UpdateAsync(user, cancellationToken);
            
            return Error.Unauthorized("Authentication.InvalidCredentials", "Invalid email or password");
        }

        // Check if user is active
        if (!user.IsActive)
            return Error.Unauthorized("Authentication.AccountDisabled", "Account is disabled");

        // Reset failed login attempts
        user.FailedLoginAttempts = 0;
        user.LockedOutUntil = null;
        user.LastLoginAt = DateTime.UtcNow;
        user.LastLoginIp = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        await unitOfWork.Repository<User>().UpdateAsync(user, cancellationToken);

        // Generate tokens
        var accessToken = authService.GenerateAccessToken(user.Id, user.Email, user.Roles, user.TenantId);
        var refreshToken = authService.GenerateRefreshToken();

        // Save refresh token
        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedByIp = user.LastLoginIp ?? string.Empty
        };
        await unitOfWork.Repository<RefreshToken>().AddAsync(refreshTokenEntity, cancellationToken);

        return new LoginResponse(
            accessToken,
            refreshToken,
            refreshTokenEntity.ExpiresAt,
            new UserInfo(user.Id, user.Email, user.FirstName, user.LastName, user.Roles)
        );
    }
}
