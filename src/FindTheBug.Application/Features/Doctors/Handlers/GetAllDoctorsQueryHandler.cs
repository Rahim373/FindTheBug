using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Doctors.DTOs;
using FindTheBug.Application.Features.Doctors.Queries;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Doctors.Handlers;

public class GetAllDoctorsQueryHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<GetAllDoctorsQuery, List<DoctorListItemDto>>
{
    public async Task<ErrorOr<List<DoctorListItemDto>>> Handle(GetAllDoctorsQuery request, CancellationToken cancellationToken)
    {
        var query = unitOfWork.Repository<Doctor>().GetQueryable()
            .Include(d => d.DoctorSpecialities)
            .ThenInclude(dsm => dsm.DoctorSpeciality)
            .AsQueryable();

        // Apply search filter if provided
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.ToLower();
            query = query.Where(d => 
                d.Name.ToLower().Contains(searchTerm) ||
                d.PhoneNumber.Contains(searchTerm) ||
                (d.Degree != null && d.Degree.ToLower().Contains(searchTerm)) ||
                (d.Office != null && d.Office.ToLower().Contains(searchTerm)) ||
                d.DoctorSpecialities.Any(dsm => dsm.DoctorSpeciality.Name.ToLower().Contains(searchTerm)));
        }

        var doctors = await query
            .OrderBy(d => d.Name)
            .Select(d => new DoctorListItemDto
            {
                Id = d.Id,
                Name = d.Name,
                Degree = d.Degree,
                PhoneNumber = d.PhoneNumber,
                SpecialityNames = d.DoctorSpecialities.Select(dsm => dsm.DoctorSpeciality.Name).ToList(),
                IsActive = d.IsActive
            })
            .ToListAsync(cancellationToken);

        return doctors;
    }
}
