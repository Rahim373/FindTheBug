using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Doctors.Commands;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Doctors.Handlers;

public class DeleteDoctorCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteDoctorCommand, bool>
{
    public async Task<ErrorOr<bool>> Handle(DeleteDoctorCommand request, CancellationToken cancellationToken)
    {
        var doctor = await unitOfWork.Repository<Doctor>().GetQueryable()
            .Include(d => d.DoctorSpecialities)
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (doctor == null)
            return Error.NotFound("Doctor.NotFound", "Doctor not found");

        // Remove speciality mappings first
        if (doctor.DoctorSpecialities.Any())
        {
            foreach (var mapping in doctor.DoctorSpecialities)
            {
                await unitOfWork.Repository<DoctorSpecialityMap>().DeleteAsync(mapping.Id, cancellationToken);
            }
        }

        // Remove the doctor
        await unitOfWork.Repository<Doctor>().DeleteAsync(doctor.Id, cancellationToken);

        return true;
    }
}
