using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Interfaces;
using DentalClinic.Application.Common.Models;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Doctors;

// ─── DTOs ─────────────────────────────────────────────────────────────
public class DoctorDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? LicenseNumber { get; set; }
    public string? Specialty { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }
}

public class CreateDoctorDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? LicenseNumber { get; set; }
    public string? Specialty { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? UserId { get; set; }
}

// ─── QUERIES ──────────────────────────────────────────────────────────
public record GetAllDoctorsQuery : IRequest<Result<IReadOnlyList<DoctorDto>>>;

public class GetAllDoctorsQueryHandler : IRequestHandler<GetAllDoctorsQuery, Result<IReadOnlyList<DoctorDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetAllDoctorsQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<IReadOnlyList<DoctorDto>>> Handle(GetAllDoctorsQuery request, CancellationToken ct)
    {
        var doctors = await _unitOfWork.Doctors.FindAsync(d => d.IsActive, ct);
        var dtos = doctors.Select(d => new DoctorDto
        {
            Id = d.Id, FirstName = d.FirstName, LastName = d.LastName,
            FullName = d.FullName, LicenseNumber = d.LicenseNumber,
            Specialty = d.Specialty, Phone = d.Phone, Email = d.Email,
            IsActive = d.IsActive
        }).ToList().AsReadOnly();

        return Result<IReadOnlyList<DoctorDto>>.Success(dtos);
    }
}

public record GetDoctorByIdQuery(Guid Id) : IRequest<Result<DoctorDto>>;

public class GetDoctorByIdQueryHandler : IRequestHandler<GetDoctorByIdQuery, Result<DoctorDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetDoctorByIdQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<DoctorDto>> Handle(GetDoctorByIdQuery request, CancellationToken ct)
    {
        var d = await _unitOfWork.Doctors.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Doctor), request.Id);

        return Result<DoctorDto>.Success(new DoctorDto
        {
            Id = d.Id, FirstName = d.FirstName, LastName = d.LastName,
            FullName = d.FullName, LicenseNumber = d.LicenseNumber,
            Specialty = d.Specialty, Phone = d.Phone, Email = d.Email,
            IsActive = d.IsActive
        });
    }
}

// ─── COMMANDS ─────────────────────────────────────────────────────────
public record CreateDoctorCommand(CreateDoctorDto Doctor) : IRequest<Result<DoctorDto>>;

public class CreateDoctorCommandHandler : IRequestHandler<CreateDoctorCommand, Result<DoctorDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public CreateDoctorCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<DoctorDto>> Handle(CreateDoctorCommand request, CancellationToken ct)
    {
        var dto = request.Doctor;
        var doctor = new Doctor
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            LicenseNumber = dto.LicenseNumber,
            Specialty = dto.Specialty,
            Phone = dto.Phone,
            Email = dto.Email,
            UserId = dto.UserId,
            IsActive = true,
            CreatedBy = _currentUser.UserId
        };

        await _unitOfWork.Doctors.AddAsync(doctor, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<DoctorDto>.Success(new DoctorDto
        {
            Id = doctor.Id, FirstName = doctor.FirstName, LastName = doctor.LastName,
            FullName = doctor.FullName, LicenseNumber = doctor.LicenseNumber,
            Specialty = doctor.Specialty, Phone = doctor.Phone, Email = doctor.Email,
            IsActive = doctor.IsActive
        }, "Doctor creado.");
    }
}

public record UpdateDoctorCommand(Guid Id, CreateDoctorDto Doctor) : IRequest<Result<DoctorDto>>;

public class UpdateDoctorCommandHandler : IRequestHandler<UpdateDoctorCommand, Result<DoctorDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public UpdateDoctorCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<DoctorDto>> Handle(UpdateDoctorCommand request, CancellationToken ct)
    {
        var doctor = await _unitOfWork.Doctors.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Doctor), request.Id);

        var dto = request.Doctor;
        doctor.FirstName = dto.FirstName;
        doctor.LastName = dto.LastName;
        doctor.LicenseNumber = dto.LicenseNumber;
        doctor.Specialty = dto.Specialty;
        doctor.Phone = dto.Phone;
        doctor.Email = dto.Email;
        doctor.LastModifiedBy = _currentUser.UserId;

        await _unitOfWork.Doctors.UpdateAsync(doctor, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<DoctorDto>.Success(new DoctorDto
        {
            Id = doctor.Id, FirstName = doctor.FirstName, LastName = doctor.LastName,
            FullName = doctor.FullName, LicenseNumber = doctor.LicenseNumber,
            Specialty = doctor.Specialty, Phone = doctor.Phone, Email = doctor.Email,
            IsActive = doctor.IsActive
        }, "Doctor actualizado.");
    }
}
