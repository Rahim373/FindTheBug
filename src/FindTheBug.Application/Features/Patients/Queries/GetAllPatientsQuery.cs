using FindTheBug.Application.Common.Messaging;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Patients.Queries;

public record GetAllPatientsQuery(string? Search = null) : IQuery<IEnumerable<Patient>>;
