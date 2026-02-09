using DentalClinic.Application.Common.Interfaces;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Doctors.DTOs;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Doctors.Commands.CreateDoctorCommand;

public record CreateDoctorCommand(CreateDoctorDto Doctor) : IRequest<Result<DoctorDto>>;

public class CreateDoctorCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<CreateDoctorCommand, Result<DoctorDto>>
{
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
            CreatedBy = currentUser.UserId
        };

        await unitOfWork.Doctors.AddAsync(doctor, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result<DoctorDto>.Success(new DoctorDto
        {
            Id = doctor.Id, FirstName = doctor.FirstName, LastName = doctor.LastName,
            FullName = doctor.FullName, LicenseNumber = doctor.LicenseNumber,
            Specialty = doctor.Specialty, Phone = doctor.Phone, Email = doctor.Email,
            IsActive = doctor.IsActive
        }, "Doctor creado.");
    }
}