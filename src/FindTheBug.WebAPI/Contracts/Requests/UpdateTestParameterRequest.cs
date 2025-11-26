namespace FindTheBug.WebAPI.Contracts.Requests;

public record UpdateTestParameterRequest(
    string ParameterName,
    string? Unit,
    decimal? ReferenceRangeMin,
    decimal? ReferenceRangeMax,
    string DataType,
    int DisplayOrder
);
