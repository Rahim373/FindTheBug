namespace FindTheBug.WebAPI.Contracts.Requests;

public record UpdateTestResultRequest(
    string ResultValue,
    bool IsAbnormal,
    string? Notes
);

public record VerifyRequest(string VerifiedBy);
