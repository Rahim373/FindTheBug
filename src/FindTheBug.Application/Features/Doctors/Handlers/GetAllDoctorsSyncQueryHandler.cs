using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Doctors.Queries;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Doctors.Handlers;

public class GetAllDoctorsSyncQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetAllDoctorsSyncQuery, PagedResult<Doctor>>
{
    public async Task<ErrorOr<Result<PagedResult<Doctor>>>> Handle(GetAllDoctorsSyncQuery request, CancellationToken cancellationToken)
    {
        var query = unitOfWork.Repository<Doctor>().GetQueryable()
            .Include(d => d.DoctorSpecialities)
            .ThenInclude(dsm => dsm.DoctorSpeciality)
            .AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);

        var doctors = await query
            .OrderBy(d => d.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        doctors.ForEach(d =>
        {
            foreach (var item in d.DoctorSpecialities)
            {
                item.DoctorSpeciality.DoctorMappings = null;
                item.Doctor = null;
            }
        });

        return Result<PagedResult<Doctor>>.Success(new PagedResult<Doctor>
        {
            Items = doctors,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        });
    }
}
