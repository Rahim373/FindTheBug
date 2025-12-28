using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Laboratory.DiagnosticTests.DTOs;

namespace FindTheBug.Application.Features.Laboratory.DiagnosticTests.Queries;

public record GetDiagnosticTestByIdQuery(Guid Id) : IQuery<DiagnosticTestResponseDto>;