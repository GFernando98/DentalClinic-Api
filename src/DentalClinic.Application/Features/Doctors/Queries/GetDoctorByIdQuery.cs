using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Doctors.DTOs;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Doctors.Queries;

public record GetDoctorByIdQuery(Guid Id) : IRequest<Result<DoctorDto>>;

public class GetDoctorByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetDoctorByIdQuery, Result<DoctorDto>>
{
    public async Task<Result<DoctorDto>> Handle(GetDoctorByIdQuery request, CancellationToken ct)
    {
        var d = await unitOfWork.Doctors.GetByIdAsync(request.Id, ct)
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