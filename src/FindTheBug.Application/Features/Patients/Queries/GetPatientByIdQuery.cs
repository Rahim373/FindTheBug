using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Patients.DTOs;

namespace FindTheBug.Application.Features.Patients.Queries;

public record GetPatientByIdQuery(Guid Id) : IQuery<PatientResponseDto>;