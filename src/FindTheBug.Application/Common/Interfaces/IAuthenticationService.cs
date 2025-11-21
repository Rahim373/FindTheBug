using System.Security.Claims;

namespace FindTheBug.Application.Common.Interfaces;

public interface IAuthenticationService
{
    string GenerateAccessToken(Guid userId, string email, string roles, string tenantId);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateToken(string token);
    Guid? GetUserIdFromToken(string token);
}
