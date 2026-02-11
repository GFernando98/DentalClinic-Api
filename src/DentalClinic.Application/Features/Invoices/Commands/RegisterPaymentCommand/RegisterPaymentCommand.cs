using System.Text.Json;
using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Interfaces;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Invoices.DTOs;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Enums;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Invoices.Commands.RegisterPaymentCommand;

public record RegisterPaymentCommand(CreatePaymentDto Payment) : IRequest<Result<PaymentDto>>;

public class RegisterPaymentCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<RegisterPaymentCommand, Result<PaymentDto>>
{
    public async Task<Result<PaymentDto>> Handle(RegisterPaymentCommand request, CancellationToken ct)
    {
        var dto = request.Payment;
        
        var invoice = await unitOfWork.Invoices.FindWithIncludeAsync(
            i => i.Id == dto.InvoiceId,
            ct,
            i => i.Payments,
            i => i.LineItems
        );

        var inv = invoice.FirstOrDefault()
            ?? throw new NotFoundException(nameof(Invoice), dto.InvoiceId);
        
        switch (inv.Status)
        {
            case InvoiceStatus.Cancelled:
                return Result<PaymentDto>.Failure("No se puede registrar pagos en una factura cancelada.");
            case InvoiceStatus.Paid:
                return Result<PaymentDto>.Failure("Esta factura ya está pagada completamente.");
        }

        var balance = inv.Total - inv.AmountPaid;
        if (dto.Amount <= 0)
        {
            return Result<PaymentDto>.Failure("El monto debe ser mayor a cero.");
        }

        if (dto.Amount > balance)
        {
            return Result<PaymentDto>.Failure($"El monto excede el saldo pendiente (L {balance:N2}).");
        }
        
        var payment = new Payment
        {
            InvoiceId = dto.InvoiceId,
            PaymentDate = DateTime.UtcNow,
            Amount = dto.Amount,
            PaymentMethod = dto.PaymentMethod,
            ReferenceNumber = dto.ReferenceNumber,
            Notes = dto.Notes,
            CreatedBy = currentUser.UserId
        };

        await unitOfWork.Payments.AddAsync(payment, ct);
        
        var newAmountPaid = inv.AmountPaid + dto.Amount;
        var newBalance = inv.Total - newAmountPaid;

        if (newBalance <= 0)
        {
            inv.Status = InvoiceStatus.Paid;
            
            await MarkTreatmentRecordsAsPaidAsync(inv, ct);
        }
        else if (newAmountPaid > 0)
        {
            inv.Status = InvoiceStatus.PartiallyPaid;
        }

        await unitOfWork.Invoices.UpdateAsync(inv, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result<PaymentDto>.Success(new PaymentDto
        {
            Id = payment.Id,
            PaymentDate = payment.PaymentDate,
            Amount = payment.Amount,
            PaymentMethod = payment.PaymentMethod,
            ReferenceNumber = payment.ReferenceNumber,
            Notes = payment.Notes
        }, "Pago registrado exitosamente.");
    }

    private async Task MarkTreatmentRecordsAsPaidAsync(Invoice invoice, CancellationToken ct)
    {
        var allTreatmentRecordIds = new List<Guid>();

        foreach (var lineItem in invoice.LineItems)
        {
            var ids = JsonSerializer.Deserialize<List<Guid>>(lineItem.TreatmentRecordIds);
            if (ids != null)
            {
                allTreatmentRecordIds.AddRange(ids);
            }
        }
        
        var treatmentRecords = await unitOfWork.TreatmentRecords.FindAsync(
            tr => allTreatmentRecordIds.Contains(tr.Id),
            ct
        );

        foreach (var tr in treatmentRecords)
        {
            tr.IsPaid = true;
            await unitOfWork.TreatmentRecords.UpdateAsync(tr, ct);
        }
    }
}