using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Patients.DTOs;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Patients.Queries;

public record GetPatientByIdQuery(Guid Id) : IRequest<Result<PatientDto>>;

public class GetPatientByIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetPatientByIdQuery, Result<PatientDto>>
{
    public async Task<Result<PatientDto>> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
    {
        var patient = await unitOfWork.Patients.GetByIdAsync(request.Id, cancellationToken)
                      ?? throw new NotFoundException(nameof(Patient), request.Id);

        return Result<PatientDto>.Success(new PatientDto
        {
            Id = patient.Id, FirstName = patient.FirstName, LastName = patient.LastName,
            FullName = patient.FullName, IdentityNumber = patient.IdentityNumber,
            DateOfBirth = patient.DateOfBirth, Age = patient.Age, Gender = patient.Gender,
            Phone = patient.Phone, WhatsAppNumber = patient.WhatsAppNumber, Email = patient.Email,
            Address = patient.Address, City = patient.City, Occupation = patient.Occupation,
            EmergencyContactName = patient.EmergencyContactName, EmergencyContactPhone = patient.EmergencyContactPhone,
            Allergies = patient.Allergies, MedicalConditions = patient.MedicalConditions,
            CurrentMedications = patient.CurrentMedications, Notes = patient.Notes,
            ProfilePhotoUrl = patient.ProfilePhotoUrl, CreatedAt = patient.CreatedAt
        });
    }
}
