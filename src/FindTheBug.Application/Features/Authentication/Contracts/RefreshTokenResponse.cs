namespace FindTheBug.Application.Features.Authentication.Contracts;

public record RefreshTokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt
);
