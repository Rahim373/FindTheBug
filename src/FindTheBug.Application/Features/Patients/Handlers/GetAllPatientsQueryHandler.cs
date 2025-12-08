using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Patients.DTOs;
using FindTheBug.Application.Features.Patients.Queries;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Patients.Handlers;

public class GetAllPatientsQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetAllPatientsQuery, List<PatientListItemDto>>
{
    public async Task<ErrorOr<List<PatientListItemDto>>> Handle(GetAllPatientsQuery request, CancellationToken cancellationToken)
    {
        var query = unitOfWork.Repository<Patient>().GetQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchLower = request.Search.ToLower();
            query = query.Where(p =>
                p.FirstName.ToLower().Contains(searchLower) ||
                p.MobileNumber.Contains(request.Search));
        }

        var patients = await query
            .OrderBy(p => p.FirstName)
            .ToListAsync(cancellationToken);

        var patientDtos = patients.Select(p => new PatientListItemDto
        {
            Id = p.Id,
            Name = p.FirstName,
            MobileNumber = p.MobileNumber,
            Age = p.Age,
            Gender = p.Gender
        }).ToList();

        return patientDtos;
    }
}
