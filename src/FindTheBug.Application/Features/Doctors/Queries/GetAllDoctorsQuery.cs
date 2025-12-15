using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Doctors.DTOs;

namespace FindTheBug.Application.Features.Doctors.Queries;

public record GetAllDoctorsQuery(string? Search = null) : ICommand<List<DoctorListItemDto>>;
