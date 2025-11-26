using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;

namespace FindTheBug.Application.Features.Authentication.Commands;

public class RequestPasswordResetCommandHandler(
    IUnitOfWork unitOfWork,
    IEmailService emailService,
    IHttpContextAccessor httpContextAccessor) 
    : ICommandHandler<RequestPasswordResetCommand, bool>
{
    public async Task<ErrorOr<bool>> Handle(RequestPasswordResetCommand request, CancellationToken cancellationToken)
    {
        // Get user by email
        var users = await unitOfWork.Repository<User>().GetAllAsync(cancellationToken);
        var user = users.FirstOrDefault(u => u.Email.ToLower() == request.Email.ToLower());

        // Always return success to prevent email enumeration
        if (user is null || !user.IsActive)
            return true;

        // Generate secure random token
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

        // Save reset token
        var resetToken = new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            Email = user.Email,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            IpAddress = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString()
        };
        await unitOfWork.Repository<PasswordResetToken>().AddAsync(resetToken, cancellationToken);

        // Send email
        await emailService.SendPasswordResetEmailAsync(user.Email, token, cancellationToken);

        return true;
    }
}
