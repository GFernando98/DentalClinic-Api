using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Models;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Enums;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Invoices.Commands.CancelInvoiceCommand;

public record CancelInvoiceCommand(Guid InvoiceId, string Reason) : IRequest<Result<bool>>;

public class CancelInvoiceCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CancelInvoiceCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(CancelInvoiceCommand request, CancellationToken ct)
    {
        var invoice = await unitOfWork.Invoices.FindWithIncludeAsync(
            i => i.Id == request.InvoiceId,
            ct,
            i => i.Payments
        );

        var inv = invoice.FirstOrDefault()
                  ?? throw new NotFoundException(nameof(Invoice), request.InvoiceId);
        
        if (inv.Payments.Any())
        {
            return Result<bool>.Failure("No se puede cancelar una factura con pagos registrados.");
        }

        inv.Status = InvoiceStatus.Cancelled;
        inv.Notes = $"{inv.Notes}\n[CANCELADA] Razón: {request.Reason}";

        await unitOfWork.Invoices.UpdateAsync(inv, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result<bool>.Success(true, "Factura cancelada exitosamente.");
    }
}