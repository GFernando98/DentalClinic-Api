using DentalClinic.Application.Common.Interfaces;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Patients.DTOs;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Patients.Commands.CreatePatientCommand;

public record CreatePatientCommand(CreatePatientDto Patient) : IRequest<Result<PatientDto>>;

public class CreatePatientCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<CreatePatientCommand, Result<PatientDto>>
{
    public async Task<Result<PatientDto>> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Patient;

        // Validate unique identity number
        if (!string.IsNullOrWhiteSpace(dto.IdentityNumber))
        {
            var exists = await unitOfWork.Patients.ExistsAsync(
                p => p.IdentityNumber == dto.IdentityNumber, cancellationToken);
            if (exists)
                return Result<PatientDto>.Failure("Ya existe un paciente con ese número de identidad.");
        }

        var patient = new Patient
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            IdentityNumber = dto.IdentityNumber,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            Phone = dto.Phone,
            WhatsAppNumber = dto.WhatsAppNumber,
            Email = dto.Email,
            Address = dto.Address,
            City = dto.City,
            Occupation = dto.Occupation,
            EmergencyContactName = dto.EmergencyContactName,
            EmergencyContactPhone = dto.EmergencyContactPhone,
            Allergies = dto.Allergies,
            MedicalConditions = dto.MedicalConditions,
            CurrentMedications = dto.CurrentMedications,
            Notes = dto.Notes,
            CreatedBy = currentUser.UserId
        };

        await unitOfWork.Patients.AddAsync(patient, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<PatientDto>.Success(MapToDto(patient), "Paciente creado exitosamente.");
    }

    private static PatientDto MapToDto(Patient p) => new()
    {
        Id = p.Id,
        FirstName = p.FirstName,
        LastName = p.LastName,
        FullName = p.FullName,
        IdentityNumber = p.IdentityNumber,
        DateOfBirth = p.DateOfBirth,
        Age = p.Age,
        Gender = p.Gender,
        Phone = p.Phone,
        WhatsAppNumber = p.WhatsAppNumber,
        Email = p.Email,
        Address = p.Address,
        City = p.City,
        Occupation = p.Occupation,
        EmergencyContactName = p.EmergencyContactName,
        EmergencyContactPhone = p.EmergencyContactPhone,
        Allergies = p.Allergies,
        MedicalConditions = p.MedicalConditions,
        CurrentMedications = p.CurrentMedications,
        Notes = p.Notes,
        CreatedAt = p.CreatedAt
    };
}
