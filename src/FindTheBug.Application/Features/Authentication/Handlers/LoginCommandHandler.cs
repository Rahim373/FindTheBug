using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Authentication.Commands;
using FindTheBug.Application.Features.Authentication.Contracts;
using FindTheBug.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Authentication.Handlers;

public class LoginCommandHandler(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IAuthenticationService authService,
    IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<LoginCommand, LoginResponse>
{
    public async Task<ErrorOr<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Get user by email or phone
        var users = await unitOfWork.Repository<User>().GetAllAsync(cancellationToken);
        var user = users.FirstOrDefault(u =>
            (u.Email != null && u.Email.ToLower() == request.EmailOrPhone.ToLower()) ||
            u.Phone == request.EmailOrPhone);

        if (user is null)
            return Error.Unauthorized("Authentication.InvalidCredentials", "Invalid email/phone or password");

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

            return Error.Unauthorized("Authentication.InvalidCredentials", "Invalid email/phone or password");
        }

        // Check if user is active
        if (!user.IsActive)
            return Error.Unauthorized("Authentication.AccountDisabled", "Account is disabled");

        // Check if user is allowed to login
        if (!user.AllowUserLogin)
            return Error.Unauthorized("Authentication.LoginNotAllowed", "User is not allowed to login");

        // Reset failed login attempts
        user.FailedLoginAttempts = 0;
        user.LockedOutUntil = null;
        user.LastLoginAt = DateTime.UtcNow;
        user.LastLoginIp = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        await unitOfWork.Repository<User>().UpdateAsync(user, cancellationToken);

        // Get user roles with permissions
        var userRolesData = await unitOfWork.Repository<UserRole>()
            .GetQueryable()
            .Include(ur => ur.Role)
                .ThenInclude(r => r.RoleModulePermissions)
                    .ThenInclude(rmp => rmp.Module)
            .Where(ur => ur.UserId == user.Id)
            .ToListAsync(cancellationToken);

        var userRoles = userRolesData.Select(ur => ur.Role?.Name).Where(n => n != null).ToList();
        var rolesString = string.Join(",", userRoles);

        // Collect all permissions from all roles
        var permissions = new List<ModulePermissionInfo>();
        foreach (var userRole in userRolesData)
        {
            if (userRole.Role?.RoleModulePermissions != null)
            {
                foreach (var roleModulePermission in userRole.Role.RoleModulePermissions)
                {
                    var module = roleModulePermission.Module;
                    if (module != null && module.IsActive)
                    {
                        if (roleModulePermission.CanView)
                            permissions.Add(new ModulePermissionInfo(module.Name, "View"));
                        if (roleModulePermission.CanCreate)
                            permissions.Add(new ModulePermissionInfo(module.Name, "Create"));
                        if (roleModulePermission.CanEdit)
                            permissions.Add(new ModulePermissionInfo(module.Name, "Edit"));
                        if (roleModulePermission.CanDelete)
                            permissions.Add(new ModulePermissionInfo(module.Name, "Delete"));
                    }
                }
            }
        }

        // Generate tokens
        var accessToken = authService.GenerateAccessToken(user.Id, user.Email ?? user.Phone, rolesString);
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
            new UserInfo(user.Id, user.Email ?? user.Phone, user.FirstName, user.LastName, rolesString, permissions.Distinct().ToList())
        );
    }
}