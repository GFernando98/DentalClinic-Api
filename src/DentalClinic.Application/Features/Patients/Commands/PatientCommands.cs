using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Patients.DTOs;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;
using DentalClinic.Application.Common.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Patients.Commands;

// ─── CREATE ───────────────────────────────────────────────────────────
public record CreatePatientCommand(CreatePatientDto Patient) : IRequest<Result<PatientDto>>;

public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, Result<PatientDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public CreatePatientCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<PatientDto>> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Patient;

        // Validate unique identity number
        if (!string.IsNullOrWhiteSpace(dto.IdentityNumber))
        {
            var exists = await _unitOfWork.Patients.ExistsAsync(
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
            CreatedBy = _currentUser.UserId
        };

        await _unitOfWork.Patients.AddAsync(patient, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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

// ─── UPDATE ───────────────────────────────────────────────────────────
public record UpdatePatientCommand(Guid Id, UpdatePatientDto Patient) : IRequest<Result<PatientDto>>;

public class UpdatePatientCommandHandler : IRequestHandler<UpdatePatientCommand, Result<PatientDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public UpdatePatientCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<PatientDto>> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = await _unitOfWork.Patients.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Patient), request.Id);

        var dto = request.Patient;

        // Validate unique identity number if changed
        if (!string.IsNullOrWhiteSpace(dto.IdentityNumber) && dto.IdentityNumber != patient.IdentityNumber)
        {
            var exists = await _unitOfWork.Patients.ExistsAsync(
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
        patient.LastModifiedBy = _currentUser.UserId;

        await _unitOfWork.Patients.UpdateAsync(patient, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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

// ─── DELETE (Soft) ────────────────────────────────────────────────────
public record DeletePatientCommand(Guid Id) : IRequest<Result<bool>>;

public class DeletePatientCommandHandler : IRequestHandler<DeletePatientCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public DeletePatientCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<bool>> Handle(DeletePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = await _unitOfWork.Patients.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Patient), request.Id);

        patient.IsDeleted = true;
        patient.DeletedAt = DateTime.UtcNow;
        patient.DeletedBy = _currentUser.UserId;

        await _unitOfWork.Patients.UpdateAsync(patient, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "Paciente eliminado.");
    }
}
