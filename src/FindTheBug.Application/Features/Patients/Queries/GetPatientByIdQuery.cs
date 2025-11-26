using FindTheBug.Application.Common.Messaging;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Patients.Queries;

public record GetPatientByIdQuery(Guid Id) : IQuery<Patient>;