using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Authentication.Commands;

public class ResetPasswordCommandHandler(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher) 
    : ICommandHandler<ResetPasswordCommand, bool>
{
    public async Task<ErrorOr<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        // Find reset token
        var resetTokens = await unitOfWork.Repository<PasswordResetToken>().GetAllAsync(cancellationToken);
        var resetToken = resetTokens.FirstOrDefault(rt => rt.Token == request.Token);

        if (resetToken is null)
            return Error.Validation("Authentication.InvalidToken", "Invalid or expired reset token");

        if (!resetToken.IsValid)
        {
            if (resetToken.IsUsed)
                return Error.Validation("Authentication.TokenUsed", "Reset token has already been used");
            
            return Error.Validation("Authentication.TokenExpired", "Reset token has expired");
        }

        // Get user
        var users = await unitOfWork.Repository<User>().GetAllAsync(cancellationToken);
        var user = users.FirstOrDefault(u => u.Email.ToLower() == resetToken.Email.ToLower());

        if (user is null)
            return Error.NotFound("User.NotFound", "User not found");

        // Validate new password
        if (request.NewPassword.Length < 8)
            return Error.Validation("Authentication.WeakPassword", "Password must be at least 8 characters long");

        // Update password
        user.PasswordHash = passwordHasher.HashPassword(request.NewPassword);
        user.FailedLoginAttempts = 0;
        user.LockedOutUntil = null;
        await unitOfWork.Repository<User>().UpdateAsync(user, cancellationToken);

        // Mark token as used
        resetToken.UsedAt = DateTime.UtcNow;
        await unitOfWork.Repository<PasswordResetToken>().UpdateAsync(resetToken, cancellationToken);

        // Revoke all refresh tokens for security
        var refreshTokens = await unitOfWork.Repository<RefreshToken>().GetAllAsync(cancellationToken);
        var userTokens = refreshTokens.Where(rt => rt.UserId == user.Id && rt.IsActive).ToList();
        
        foreach (var token in userTokens)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.ReasonRevoked = "Password reset";
            await unitOfWork.Repository<RefreshToken>().UpdateAsync(token, cancellationToken);
        }

        return true;
    }
}
