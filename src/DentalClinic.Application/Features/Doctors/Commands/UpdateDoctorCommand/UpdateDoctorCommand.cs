using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Interfaces;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Doctors.DTOs;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Doctors.Commands.UpdateDoctorCommand;

public record UpdateDoctorCommand(Guid Id, CreateDoctorDto Doctor) : IRequest<Result<DoctorDto>>;

public class UpdateDoctorCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<UpdateDoctorCommand, Result<DoctorDto>>
{
    public async Task<Result<DoctorDto>> Handle(UpdateDoctorCommand request, CancellationToken ct)
    {
        var doctor = await unitOfWork.Doctors.GetByIdAsync(request.Id, ct)
                     ?? throw new NotFoundException(nameof(Doctor), request.Id);

        var dto = request.Doctor;
        doctor.FirstName = dto.FirstName;
        doctor.LastName = dto.LastName;
        doctor.LicenseNumber = dto.LicenseNumber;
        doctor.Specialty = dto.Specialty;
        doctor.Phone = dto.Phone;
        doctor.Email = dto.Email;
        doctor.LastModifiedBy = currentUser.UserId;

        await unitOfWork.Doctors.UpdateAsync(doctor, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result<DoctorDto>.Success(new DoctorDto
        {
            Id = doctor.Id, FirstName = doctor.FirstName, LastName = doctor.LastName,
            FullName = doctor.FullName, LicenseNumber = doctor.LicenseNumber,
            Specialty = doctor.Specialty, Phone = doctor.Phone, Email = doctor.Email,
            IsActive = doctor.IsActive
        }, "Doctor actualizado.");
    }
}
