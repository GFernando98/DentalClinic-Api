using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Doctors.DTOs;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Doctors.Queries;

public record GetAllDoctorsQuery : IRequest<Result<IReadOnlyList<DoctorDto>>>;

public class GetAllDoctorsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetAllDoctorsQuery, Result<IReadOnlyList<DoctorDto>>>
{
    public async Task<Result<IReadOnlyList<DoctorDto>>> Handle(GetAllDoctorsQuery request, CancellationToken ct)
    {
        var doctors = await unitOfWork.Doctors.FindAsync(d => d.IsActive, ct);
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