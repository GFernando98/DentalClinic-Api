using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Patients.DTOs;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Patients.Queries;

public record GetAllPatientsQuery : IRequest<Result<IReadOnlyList<PatientDto>>>;

public class GetAllPatientsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetAllPatientsQuery, Result<IReadOnlyList<PatientDto>>>
{
    public async Task<Result<IReadOnlyList<PatientDto>>> Handle(GetAllPatientsQuery request, CancellationToken cancellationToken)
    {
        var patients = await unitOfWork.Patients.GetAllAsync(cancellationToken);
        var dtos = patients.Select(MapToDto).ToList().AsReadOnly();
        return Result<IReadOnlyList<PatientDto>>.Success(dtos);
    }

    private static PatientDto MapToDto(Patient p) => new()
    {
        Id = p.Id, FirstName = p.FirstName, LastName = p.LastName,
        FullName = p.FullName, IdentityNumber = p.IdentityNumber,
        DateOfBirth = p.DateOfBirth, Age = p.Age, Gender = p.Gender,
        Phone = p.Phone, WhatsAppNumber = p.WhatsAppNumber, Email = p.Email,
        Address = p.Address, City = p.City, CreatedAt = p.CreatedAt
    };
}