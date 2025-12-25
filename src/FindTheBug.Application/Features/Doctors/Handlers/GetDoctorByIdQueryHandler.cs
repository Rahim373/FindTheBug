using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Doctors.DTOs;
using FindTheBug.Application.Features.Doctors.Queries;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Doctors.Handlers;

public class GetDoctorByIdQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetDoctorByIdQuery, DoctorResponseDto>
{
    public async Task<ErrorOr<DoctorResponseDto>> Handle(GetDoctorByIdQuery request, CancellationToken cancellationToken)
    {
        var doctor = await unitOfWork.Repository<Doctor>().GetQueryable()
            .Include(d => d.DoctorSpecialities)
            .ThenInclude(dsm => dsm.DoctorSpeciality)
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (doctor == null)
            return Error.NotFound("Doctor.NotFound", "Doctor not found");

        var specialities = doctor.DoctorSpecialities
            .Select(dsm => new DoctorSpecialityDto
            {
                Id = dsm.DoctorSpeciality.Id,
                Name = dsm.DoctorSpeciality.Name,
                Description = dsm.DoctorSpeciality.Description,
                IsActive = dsm.DoctorSpeciality.IsActive
            }).ToList();

        return new DoctorResponseDto
        {
            Id = doctor.Id,
            Name = doctor.Name,
            Degree = doctor.Degree,
            Office = doctor.Office,
            PhoneNumber = doctor.PhoneNumber,
            Specialities = specialities,
            IsActive = doctor.IsActive,
            CreatedAt = doctor.CreatedAt,
            UpdatedAt = doctor.UpdatedAt
        };
    }
}
