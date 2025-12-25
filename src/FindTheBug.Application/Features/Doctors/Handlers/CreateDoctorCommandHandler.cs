using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Doctors.Commands;
using FindTheBug.Application.Features.Doctors.DTOs;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Doctors.Handlers;

public class CreateDoctorCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<CreateDoctorCommand, DoctorResponseDto>
{
    public async Task<ErrorOr<DoctorResponseDto>> Handle(CreateDoctorCommand request, CancellationToken cancellationToken)
    {
        // Check if doctor with phone number exists
        var existing = await unitOfWork.Repository<Doctor>().GetQueryable()
            .FirstOrDefaultAsync(d => d.PhoneNumber == request.PhoneNumber, cancellationToken);

        if (existing != null)
            return Error.Conflict("Doctor.PhoneExists", "Doctor with this phone number already exists");

        // Validate speciality IDs
        if (request.SpecialityIds.Any())
        {
            var specialityIds = await unitOfWork.Repository<DoctorSpeciality>().GetQueryable()
                .Where(ds => request.SpecialityIds.Contains(ds.Id) && ds.IsActive)
                .Select(ds => ds.Id)
                .ToListAsync(cancellationToken);

            var invalidIds = request.SpecialityIds.Except(specialityIds).ToList();
            if (invalidIds.Any())
                return Error.Validation("Doctor.InvalidSpecialities", "Invalid speciality IDs provided");
        }

        var doctor = new Doctor
        {
            Name = request.Name,
            Degree = request.Degree,
            Office = request.Office,
            PhoneNumber = request.PhoneNumber,
            IsActive = true
        };

        var created = await unitOfWork.Repository<Doctor>().AddAsync(doctor, cancellationToken);

        // Add speciality mappings
        if (request.SpecialityIds.Any())
        {
            foreach (var specialityId in request.SpecialityIds)
            {
                var mapping = new DoctorSpecialityMap
                {
                    DoctorId = created.Id,
                    DoctorSpecialityId = specialityId
                };
                await unitOfWork.Repository<DoctorSpecialityMap>().AddAsync(mapping, cancellationToken);
            }
        }

        // Get the created doctor with specialities
        var doctorWithSpecialities = await unitOfWork.Repository<Doctor>().GetQueryable()
            .Include(d => d.DoctorSpecialities)
            .ThenInclude(dsm => dsm.DoctorSpeciality)
            .FirstAsync(d => d.Id == created.Id, cancellationToken);

        var specialities = doctorWithSpecialities.DoctorSpecialities
            .Select(dsm => new DoctorSpecialityDto
            {
                Id = dsm.DoctorSpeciality.Id,
                Name = dsm.DoctorSpeciality.Name,
                Description = dsm.DoctorSpeciality.Description,
                IsActive = dsm.DoctorSpeciality.IsActive
            }).ToList();

        return new DoctorResponseDto
        {
            Id = created.Id,
            Name = created.Name,
            Degree = created.Degree,
            Office = created.Office,
            PhoneNumber = created.PhoneNumber,
            Specialities = specialities,
            IsActive = created.IsActive,
            CreatedAt = created.CreatedAt,
            UpdatedAt = created.UpdatedAt
        };
    }
}
