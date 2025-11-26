using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Authentication.Commands;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Authentication.Handlers;

public class ChangePasswordCommandHandler(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher) 
    : ICommandHandler<ChangePasswordCommand, bool>
{
    public async Task<ErrorOr<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        // Get user
        var user = await unitOfWork.Repository<User>().GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Error.NotFound("User.NotFound", "User not found");

        // Verify current password
        if (!passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash))
            return Error.Validation("Authentication.InvalidPassword", "Current password is incorrect");

        // Validate new password
        if (request.NewPassword.Length < 8)
            return Error.Validation("Authentication.WeakPassword", "Password must be at least 8 characters long");

        if (request.NewPassword == request.CurrentPassword)
            return Error.Validation("Authentication.SamePassword", "New password must be different from current password");

        // Hash and update password
        user.PasswordHash = passwordHasher.HashPassword(request.NewPassword);
        await unitOfWork.Repository<User>().UpdateAsync(user, cancellationToken);

        // Revoke all refresh tokens for security
        var refreshTokens = await unitOfWork.Repository<RefreshToken>().GetAllAsync(cancellationToken);
        var userTokens = refreshTokens.Where(rt => rt.UserId == user.Id && rt.IsActive).ToList();
        
        foreach (var token in userTokens)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.ReasonRevoked = "Password changed";
            await unitOfWork.Repository<RefreshToken>().UpdateAsync(token, cancellationToken);
        }

        return true;
    }
}
