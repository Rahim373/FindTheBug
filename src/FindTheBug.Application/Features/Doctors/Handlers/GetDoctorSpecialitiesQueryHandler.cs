using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Doctors.DTOs;
using FindTheBug.Application.Features.Doctors.Queries;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Doctors.Handlers;

public class GetDoctorSpecialitiesQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetDoctorSpecialitiesQuery, List<DoctorSpecialityDto>>
{
    public async Task<ErrorOr<List<DoctorSpecialityDto>>> Handle(GetDoctorSpecialitiesQuery request, CancellationToken cancellationToken)
    {
        var specialities = await unitOfWork.Repository<DoctorSpeciality>().GetQueryable()
            .Where(ds => ds.IsActive)
            .OrderBy(ds => ds.Name)
            .Select(ds => new DoctorSpecialityDto
            {
                Id = ds.Id,
                Name = ds.Name,
                Description = ds.Description,
                IsActive = ds.IsActive
            })
            .ToListAsync(cancellationToken);

        return specialities;
    }
}
