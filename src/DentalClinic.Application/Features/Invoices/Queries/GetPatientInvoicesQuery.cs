using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Invoices.DTOs;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Invoices.Queries;

public record GetPatientInvoicesQuery(Guid PatientId) : IRequest<Result<IReadOnlyList<InvoiceDto>>>;

public class GetPatientInvoicesQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetPatientInvoicesQuery, Result<IReadOnlyList<InvoiceDto>>>
{
    public async Task<Result<IReadOnlyList<InvoiceDto>>> Handle(
        GetPatientInvoicesQuery request, 
        CancellationToken ct)
    {
        var invoices = await unitOfWork.Invoices.FindWithIncludeAsync(
            i => i.PatientId == request.PatientId,
            ct,
            i => i.Patient,
            i => i.LineItems,
            i => i.Payments,
            i => i.TaxInformation
        );

        var dtos = invoices
            .OrderByDescending(i => i.InvoiceDate)
            .Select(invoice => new InvoiceDto
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                InvoiceType = invoice.InvoiceType,
                PatientId = invoice.PatientId,
                PatientName = invoice.Patient.FullName,
                OdontogramId = invoice.OdontogramId,
                InvoiceDate = invoice.InvoiceDate,
                DueDate = invoice.DueDate,
                Subtotal = invoice.Subtotal,
                Tax = invoice.Tax,
                Discount = invoice.Discount,
                Total = invoice.Total,
                AmountPaid = invoice.AmountPaid,
                Balance = invoice.Balance,
                Notes = invoice.Notes,
                Status = invoice.Status,
                CAI = invoice.TaxInformation?.CAI,
                LineItems = invoice.LineItems.Select(li => new InvoiceLineDto
                {
                    Id = li.Id,
                    Description = li.Description,
                    ToothNumbers = li.ToothNumbers,
                    IsGlobalTreatment = li.IsGlobalTreatment,
                    Quantity = li.Quantity,
                    UnitPrice = li.UnitPrice,
                    Subtotal = li.Subtotal
                }).ToList(),
                Payments = invoice.Payments.Select(p => new PaymentDto
                {
                    Id = p.Id,
                    PaymentDate = p.PaymentDate,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    ReferenceNumber = p.ReferenceNumber,
                    Notes = p.Notes
                }).ToList()
            })
            .ToList()
            .AsReadOnly();

        return Result<IReadOnlyList<InvoiceDto>>.Success(dtos);
    }
}