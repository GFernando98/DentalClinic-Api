using DentalClinic.Application.Common.Interfaces;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.TaxInformation.DTOs;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.TaxInformation.Commands.CreateTaxInformationCommand;

public record CreateTaxInformationCommand(CreateTaxInformationDto TaxInfo) 
    : IRequest<Result<TaxInformationDto>>;

public class CreateTaxInformationCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<CreateTaxInformationCommand, Result<TaxInformationDto>>
{
    public async Task<Result<TaxInformationDto>> Handle(
        CreateTaxInformationCommand request, 
        CancellationToken ct)
    {
        var dto = request.TaxInfo;
        
        if (dto.RangeStart >= dto.RangeEnd)
        {
            return Result<TaxInformationDto>.Failure("El rango inicial debe ser menor que el rango final.");
        }
        
        var existingCai = await unitOfWork.TaxInformation.FindAsync(
            t => t.CAI == dto.CAI && t.IsActive, ct);

        if (existingCai.Any())
        {
            return Result<TaxInformationDto>.Failure("Ya existe un CAI activo con ese código.");
        }

        var taxInfo = new Domain.Entities.TaxInformation
        {
            CAI = dto.CAI,
            InvoiceType = dto.InvoiceType,
            RangeStart = dto.RangeStart,
            RangeEnd = dto.RangeEnd,
            CurrentNumber = dto.RangeStart, 
            AuthorizationDate = dto.AuthorizationDate,
            ExpirationDate = dto.ExpirationDate,
            IsActive = true,
            CreatedBy = currentUser.UserId
        };

        await unitOfWork.TaxInformation.AddAsync(taxInfo, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result<TaxInformationDto>.Success(new TaxInformationDto
        {
            Id = taxInfo.Id,
            CAI = taxInfo.CAI,
            InvoiceType = taxInfo.InvoiceType,
            RangeStart = taxInfo.RangeStart,
            RangeEnd = taxInfo.RangeEnd,
            CurrentNumber = taxInfo.CurrentNumber,
            AuthorizationDate = taxInfo.AuthorizationDate,
            ExpirationDate = taxInfo.ExpirationDate,
            IsActive = taxInfo.IsActive,
            IsExpired = taxInfo.IsExpired,
            IsExhausted = taxInfo.IsExhausted,
            CanGenerateInvoice = taxInfo.CanGenerateInvoice,
            RemainingInvoices = taxInfo.RemainingInvoices
        }, "CAI creado exitosamente.");
    }
}