using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Doctors.DTOs;

namespace FindTheBug.Application.Features.Doctors.Queries;

public record GetAllDoctorsQuery(
    string? Search,
    int PageNumber,
    int PageSize
) : IQuery<PagedResult<DoctorListItemDto>>;
