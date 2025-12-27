using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Doctors.Queries;

public record GetAllDoctorsSyncQuery(
    int PageNumber,
    int PageSize
) : IQuery<PagedResult<Doctor>>;
