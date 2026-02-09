using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Patients.DTOs;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Patients.Queries;

public record SearchPatientsQuery(string? Name, string? IdentityNumber) : IRequest<Result<IReadOnlyList<PatientDto>>>;

public class SearchPatientsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<SearchPatientsQuery, Result<IReadOnlyList<PatientDto>>>
{
    public async Task<Result<IReadOnlyList<PatientDto>>> Handle(SearchPatientsQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Patient> patients;

        if (!string.IsNullOrWhiteSpace(request.IdentityNumber))
        {
            patients = await unitOfWork.Patients.FindAsync(
                p => p.IdentityNumber != null && p.IdentityNumber.Contains(request.IdentityNumber), cancellationToken);
        }
        else if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var search = request.Name.ToLower();
            patients = await unitOfWork.Patients.FindAsync(
                p => p.FirstName.ToLower().Contains(search) || p.LastName.ToLower().Contains(search), cancellationToken);
        }
        else
        {
            patients = await unitOfWork.Patients.GetAllAsync(cancellationToken);
        }

        var dtos = patients.Select(p => new PatientDto
        {
            Id = p.Id, FirstName = p.FirstName, LastName = p.LastName,
            FullName = p.FullName, IdentityNumber = p.IdentityNumber,
            Phone = p.Phone, Email = p.Email, City = p.City, CreatedAt = p.CreatedAt
        }).ToList().AsReadOnly();

        return Result<IReadOnlyList<PatientDto>>.Success(dtos);
    }
}