using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Doctors.Commands;
using FindTheBug.Application.Features.Doctors.DTOs;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Doctors.Handlers;

public class UpdateDoctorCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateDoctorCommand, DoctorResponseDto>
{
    public async Task<ErrorOr<Result<DoctorResponseDto>>> Handle(UpdateDoctorCommand request, CancellationToken cancellationToken)
    {
        var doctor = await unitOfWork.Repository<Doctor>().GetQueryable()
            .Include(d => d.DoctorSpecialities)
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (doctor == null)
            return Error.NotFound("Doctor.NotFound", "Doctor not found");

        // Check if another doctor has the same phone number
        var existingWithPhone = await unitOfWork.Repository<Doctor>().GetQueryable()
            .FirstOrDefaultAsync(d => d.PhoneNumber == request.PhoneNumber && d.Id != request.Id, cancellationToken);

        if (existingWithPhone != null)
            return Error.Conflict("Doctor.PhoneExists", "Another doctor with this phone number already exists");

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

        // Update doctor properties
        doctor.Name = request.Name;
        doctor.Degree = request.Degree;
        doctor.Office = request.Office;
        doctor.PhoneNumber = request.PhoneNumber;
        doctor.IsActive = request.IsActive;

        // Update speciality mappings
        var currentSpecialityIds = doctor.DoctorSpecialities.Select(dsm => dsm.DoctorSpecialityId).ToList();
        var specialitiesToAdd = request.SpecialityIds.Except(currentSpecialityIds).ToList();
        var specialitiesToRemove = currentSpecialityIds.Except(request.SpecialityIds).ToList();

        // Remove old mappings
        if (specialitiesToRemove.Any())
        {
            var mappingsToRemove = doctor.DoctorSpecialities
                .Where(dsm => specialitiesToRemove.Contains(dsm.DoctorSpecialityId))
                .ToList();

            foreach (var mapping in mappingsToRemove)
            {
                await unitOfWork.Repository<DoctorSpecialityMap>().DeleteAsync(mapping.Id, cancellationToken);
            }
        }

        // Add new mappings
        if (specialitiesToAdd.Any())
        {
            foreach (var specialityId in specialitiesToAdd)
            {
                var newMapping = new DoctorSpecialityMap
                {
                    DoctorId = doctor.Id,
                    DoctorSpecialityId = specialityId
                };
                await unitOfWork.Repository<DoctorSpecialityMap>().AddAsync(newMapping, cancellationToken);
            }
        }

        await unitOfWork.Repository<Doctor>().UpdateAsync(doctor, cancellationToken);

        // Get the updated doctor with specialities
        var updatedDoctorWithSpecialities = await unitOfWork.Repository<Doctor>().GetQueryable()
            .Include(d => d.DoctorSpecialities)
            .ThenInclude(dsm => dsm.DoctorSpeciality)
            .FirstAsync(d => d.Id == doctor.Id, cancellationToken);

        var specialities = updatedDoctorWithSpecialities.DoctorSpecialities
            .Select(dsm => new DoctorSpecialityDto
            {
                Id = dsm.DoctorSpeciality.Id,
                Name = dsm.DoctorSpeciality.Name,
                Description = dsm.DoctorSpeciality.Description,
                IsActive = dsm.DoctorSpeciality.IsActive
            }).ToList();

        return Result<DoctorResponseDto>.Success(new DoctorResponseDto
        {
            Id = updatedDoctorWithSpecialities.Id,
            Name = updatedDoctorWithSpecialities.Name,
            Degree = updatedDoctorWithSpecialities.Degree,
            Office = updatedDoctorWithSpecialities.Office,
            PhoneNumber = updatedDoctorWithSpecialities.PhoneNumber,
            Specialities = specialities,
            IsActive = updatedDoctorWithSpecialities.IsActive,
            CreatedAt = updatedDoctorWithSpecialities.CreatedAt,
            UpdatedAt = updatedDoctorWithSpecialities.UpdatedAt
        });
    }
}
