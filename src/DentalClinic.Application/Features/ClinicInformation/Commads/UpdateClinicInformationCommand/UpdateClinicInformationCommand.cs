using DentalClinic.Application.Common.Interfaces;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.ClinicInformation.DTOs;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.ClinicInformation.Commads.UpdateClinicInformationCommand;

public record UpdateClinicInformationCommand(UpdateClinicInformationDto Clinic) 
    : IRequest<Result<ClinicInformationDto>>;

public class UpdateClinicInformationCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<UpdateClinicInformationCommand, Result<ClinicInformationDto>>
{
    public async Task<Result<ClinicInformationDto>> Handle(
        UpdateClinicInformationCommand request, 
        CancellationToken ct)
    {
        var dto = request.Clinic;
        
        var clinicInfo = (await unitOfWork.ClinicInformation.FindAsync(c => c.IsActive, ct)).FirstOrDefault();

        if (clinicInfo == null)
        {
            
            clinicInfo = new Domain.Entities.ClinicInformation
            {
                ClinicName = dto.ClinicName,
                LegalName = dto.LegalName,
                RTN = dto.RTN,
                Address = dto.Address,
                City = dto.City,
                Department = dto.Department,
                Country = dto.Country,
                Phone = dto.Phone,
                Email = dto.Email,
                Website = dto.Website,
                Logo = dto.Logo,
                IsActive = true,
                CreatedBy = currentUser.UserId
            };

            await unitOfWork.ClinicInformation.AddAsync(clinicInfo, ct);
        }
        else
        {
            clinicInfo.ClinicName = dto.ClinicName;
            clinicInfo.LegalName = dto.LegalName;
            clinicInfo.RTN = dto.RTN;
            clinicInfo.Address = dto.Address;
            clinicInfo.City = dto.City;
            clinicInfo.Department = dto.Department;
            clinicInfo.Country = dto.Country;
            clinicInfo.Phone = dto.Phone;
            clinicInfo.Email = dto.Email;
            clinicInfo.Website = dto.Website;
            clinicInfo.Logo = dto.Logo;

            await unitOfWork.ClinicInformation.UpdateAsync(clinicInfo, ct);
        }

        await unitOfWork.SaveChangesAsync(ct);

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
        }, "Información de la clínica actualizada.");
    }
}