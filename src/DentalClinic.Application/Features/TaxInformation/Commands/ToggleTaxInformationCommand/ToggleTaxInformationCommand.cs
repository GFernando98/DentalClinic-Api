using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.TaxInformation.DTOs;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.TaxInformation.Commands.ToggleTaxInformationCommand;

public record ToggleTaxInformationCommand(Guid Id, bool Activate)
    : IRequest<Result<TaxInformationDto>>;

public class ToggleTaxInformationCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ToggleTaxInformationCommand, Result<TaxInformationDto>>
{
    public async Task<Result<TaxInformationDto>> Handle(
        ToggleTaxInformationCommand request,
        CancellationToken ct)
    {
        var taxInfo = await unitOfWork.TaxInformation.GetByIdAsync(request.Id, ct)
                      ?? throw new NotFoundException(nameof(Domain.Entities.TaxInformation), request.Id);

        if (request.Activate)
        {
            // ACTIVAR

            // 1. Validar que no haya sido usado antes
            if (taxInfo.HasBeenUsed)
            {
                return Result<TaxInformationDto>.Failure(
                    "No se puede activar un CAI que ya fue utilizado anteriormente.");
            }

            // 2. Validar que no esté expirado o agotado
            if (taxInfo.IsExpired)
            {
                return Result<TaxInformationDto>.Failure(
                    "No se puede activar un CAI expirado.");
            }

            if (taxInfo.IsExhausted)
            {
                return Result<TaxInformationDto>.Failure(
                    "No se puede activar un CAI agotado.");
            }

            // 3. Desactivar cualquier otro CAI activo del mismo tipo
            var activeTaxInfos = await unitOfWork.TaxInformation.FindAsync(
                t => t.IsActive && t.InvoiceType == taxInfo.InvoiceType && t.Id != taxInfo.Id,
                ct
            );

            foreach (var activeTax in activeTaxInfos)
            {
                activeTax.IsActive = false;
                await unitOfWork.TaxInformation.UpdateAsync(activeTax, ct);
            }

            // 4. Activar el CAI solicitado
            taxInfo.IsActive = true;
        }
        else
        {
            // DESACTIVAR
            taxInfo.IsActive = false;
        }

        await unitOfWork.TaxInformation.UpdateAsync(taxInfo, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result<TaxInformationDto>.Success(new TaxInformationDto
        {
            Id = taxInfo.Id,
            CAI = taxInfo.CAI,
            InvoiceType = taxInfo.InvoiceType,
            RangeStart = taxInfo.RangeStart,
            RangeEnd = taxInfo.RangeEnd,
            Branch = taxInfo.Branch,
            PointEmission = taxInfo.PointEmission,
            CurrentNumber = taxInfo.CurrentNumber,
            AuthorizationDate = taxInfo.AuthorizationDate,
            ExpirationDate = taxInfo.ExpirationDate,
            IsActive = taxInfo.IsActive,
            HasBeenUsed = taxInfo.HasBeenUsed,
            IsExpired = taxInfo.IsExpired,
            IsExhausted = taxInfo.IsExhausted,
            CanGenerateInvoice = taxInfo.CanGenerateInvoice,
            RemainingInvoices = taxInfo.RemainingInvoices
        }, taxInfo.IsActive ? "CAI activado exitosamente." : "CAI desactivado exitosamente.");
    }
}