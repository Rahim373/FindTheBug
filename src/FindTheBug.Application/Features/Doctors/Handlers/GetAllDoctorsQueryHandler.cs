using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Doctors.DTOs;
using FindTheBug.Application.Features.Doctors.Queries;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Doctors.Handlers;

public class GetAllDoctorsQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetAllDoctorsQuery, PagedResult<DoctorListItemDto>>
{
    public async Task<ErrorOr<PagedResult<DoctorListItemDto>>> Handle(GetAllDoctorsQuery request, CancellationToken cancellationToken)
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

        var totalCount = await query.CountAsync(cancellationToken);

        var doctors = await query
            .OrderBy(d => d.Name)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
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

        return new PagedResult<DoctorListItemDto>
        {
            Items = doctors,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
