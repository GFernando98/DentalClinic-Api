using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.ClinicInformation.DTOs;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.ClinicInformation.Queries;

public record GetClinicInformationQuery : IRequest<Result<ClinicInformationDto>>;

public class GetClinicInformationQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetClinicInformationQuery, Result<ClinicInformationDto>>
{
    public async Task<Result<ClinicInformationDto>> Handle(GetClinicInformationQuery request, CancellationToken ct)
    {
        var clinicInfo = (await unitOfWork.ClinicInformation.FindAsync(c => c.IsActive, ct)).FirstOrDefault();

        if (clinicInfo == null)
        {
            return Result<ClinicInformationDto>.Failure("No se ha configurado la información de la clínica.");
        }

        return Result<ClinicInformationDto>.Success(new ClinicInformationDto
        {
            Id = clinicInfo.Id,
            ClinicName = clinicInfo.ClinicName,
            LegalName = clinicInfo.LegalName,
            RTN = clinicInfo.RTN,
            Address = clinicInfo.Address,
            City = clinicInfo.City,
            Department = clinicInfo.Department,
            Country = clinicInfo.Country,
            Phone = clinicInfo.Phone,
            Email = clinicInfo.Email,
            Website = clinicInfo.Website,
            Logo = clinicInfo.Logo,
            IsActive = clinicInfo.IsActive
        });
    }
}