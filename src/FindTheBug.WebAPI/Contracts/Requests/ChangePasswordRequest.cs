namespace FindTheBug.WebAPI.Contracts.Requests;

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
