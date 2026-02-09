using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Interfaces;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Patients.DTOs;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Patients.Commands.UpdatePatientCommand;

public record UpdatePatientCommand(Guid Id, UpdatePatientDto Patient) : IRequest<Result<PatientDto>>;

public class UpdatePatientCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<UpdatePatientCommand, Result<PatientDto>>
{
    public async Task<Result<PatientDto>> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = await unitOfWork.Patients.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Patient), request.Id);

        var dto = request.Patient;

        // Validate unique identity number if changed
        if (!string.IsNullOrWhiteSpace(dto.IdentityNumber) && dto.IdentityNumber != patient.IdentityNumber)
        {
            var exists = await unitOfWork.Patients.ExistsAsync(
                p => p.IdentityNumber == dto.IdentityNumber && p.Id != request.Id, cancellationToken);
            if (exists)
                return Result<PatientDto>.Failure("Ya existe un paciente con ese número de identidad.");
        }

        patient.FirstName = dto.FirstName;
        patient.LastName = dto.LastName;
        patient.IdentityNumber = dto.IdentityNumber;
        patient.DateOfBirth = dto.DateOfBirth;
        patient.Gender = dto.Gender;
        patient.Phone = dto.Phone;
        patient.WhatsAppNumber = dto.WhatsAppNumber;
        patient.Email = dto.Email;
        patient.Address = dto.Address;
        patient.City = dto.City;
        patient.Occupation = dto.Occupation;
        patient.EmergencyContactName = dto.EmergencyContactName;
        patient.EmergencyContactPhone = dto.EmergencyContactPhone;
        patient.Allergies = dto.Allergies;
        patient.MedicalConditions = dto.MedicalConditions;
        patient.CurrentMedications = dto.CurrentMedications;
        patient.Notes = dto.Notes;
        patient.LastModifiedBy = currentUser.UserId;

        await unitOfWork.Patients.UpdateAsync(patient, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<PatientDto>.Success(new PatientDto
        {
            Id = patient.Id,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            FullName = patient.FullName,
            IdentityNumber = patient.IdentityNumber,
            DateOfBirth = patient.DateOfBirth,
            Age = patient.Age,
            Gender = patient.Gender,
            Phone = patient.Phone,
            WhatsAppNumber = patient.WhatsAppNumber,
            Email = patient.Email,
            Address = patient.Address,
            City = patient.City,
            Occupation = patient.Occupation,
            EmergencyContactName = patient.EmergencyContactName,
            EmergencyContactPhone = patient.EmergencyContactPhone,
            Allergies = patient.Allergies,
            MedicalConditions = patient.MedicalConditions,
            CurrentMedications = patient.CurrentMedications,
            Notes = patient.Notes,
            CreatedAt = patient.CreatedAt
        }, "Paciente actualizado.");
    }
}
