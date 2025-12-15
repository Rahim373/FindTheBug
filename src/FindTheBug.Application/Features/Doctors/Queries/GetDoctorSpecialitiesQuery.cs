using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Doctors.DTOs;

namespace FindTheBug.Application.Features.Doctors.Queries;

public record GetDoctorSpecialitiesQuery() : ICommand<List<DoctorSpecialityDto>>;
