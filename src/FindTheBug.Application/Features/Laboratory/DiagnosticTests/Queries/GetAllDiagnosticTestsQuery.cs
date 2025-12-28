using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Laboratory.DiagnosticTests.DTOs;

namespace FindTheBug.Application.Features.Laboratory.DiagnosticTests.Queries;

public record GetAllDiagnosticTestsQuery(
    string? Search = null,
    string? Category = null,
    bool? IsActive = null,
    int PageNumber = 1,
    int PageSize = 10
) : IQuery<PagedResult<DiagnosticTestResponseDto>>;