using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Patients.DTOs;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Patients.Queries;

// ─── GET ALL ──────────────────────────────────────────────────────────
public record GetAllPatientsQuery : IRequest<Result<IReadOnlyList<PatientDto>>>;

public class GetAllPatientsQueryHandler : IRequestHandler<GetAllPatientsQuery, Result<IReadOnlyList<PatientDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllPatientsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<PatientDto>>> Handle(GetAllPatientsQuery request, CancellationToken cancellationToken)
    {
        var patients = await _unitOfWork.Patients.GetAllAsync(cancellationToken);
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

// ─── GET BY ID ────────────────────────────────────────────────────────
public record GetPatientByIdQuery(Guid Id) : IRequest<Result<PatientDto>>;

public class GetPatientByIdQueryHandler : IRequestHandler<GetPatientByIdQuery, Result<PatientDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPatientByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PatientDto>> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
    {
        var patient = await _unitOfWork.Patients.GetByIdAsync(request.Id, cancellationToken)
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

// ─── SEARCH ───────────────────────────────────────────────────────────
public record SearchPatientsQuery(string? Name, string? IdentityNumber) : IRequest<Result<IReadOnlyList<PatientDto>>>;

public class SearchPatientsQueryHandler : IRequestHandler<SearchPatientsQuery, Result<IReadOnlyList<PatientDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public SearchPatientsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<PatientDto>>> Handle(SearchPatientsQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Patient> patients;

        if (!string.IsNullOrWhiteSpace(request.IdentityNumber))
        {
            patients = await _unitOfWork.Patients.FindAsync(
                p => p.IdentityNumber != null && p.IdentityNumber.Contains(request.IdentityNumber), cancellationToken);
        }
        else if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var search = request.Name.ToLower();
            patients = await _unitOfWork.Patients.FindAsync(
                p => p.FirstName.ToLower().Contains(search) || p.LastName.ToLower().Contains(search), cancellationToken);
        }
        else
        {
            patients = await _unitOfWork.Patients.GetAllAsync(cancellationToken);
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
