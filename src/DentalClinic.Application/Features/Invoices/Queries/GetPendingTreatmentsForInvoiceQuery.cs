using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Invoices.DTOs;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Invoices.Queries;

public record GetPendingTreatmentsForInvoiceQuery(Guid OdontogramId) 
    : IRequest<Result<InvoicePreviewDto>>;

public class GetPendingTreatmentsForInvoiceQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetPendingTreatmentsForInvoiceQuery, Result<InvoicePreviewDto>>
{
    public async Task<Result<InvoicePreviewDto>> Handle(
        GetPendingTreatmentsForInvoiceQuery request, 
        CancellationToken ct)
    {
        // Obtener TODOS los TreatmentRecords del odontograma (globales y por diente)
        var treatmentRecords = await unitOfWork.TreatmentRecords.FindWithIncludeAsync(
            tr => tr.OdontogramId == request.OdontogramId && !tr.IsPaid,
            ct,
            tr => tr.Treatment,
            tr => tr.ToothRecord
        );

        // Separar globales de por-diente
        var globalTreatments = treatmentRecords
            .Where(tr => tr.ToothRecordId == null)
            .Select(tr => new InvoiceLineItemDto
            {
                TreatmentRecordId = tr.Id,
                TreatmentName = tr.Treatment.Name,
                IsGlobal = true,
                ToothNumbers = null,
                Quantity = 1,
                UnitPrice = tr.Price,
                Subtotal = tr.Price
            }).ToList();

        var toothTreatments = treatmentRecords
            .Where(tr => tr.ToothRecordId != null)
            .GroupBy(tr => tr.TreatmentId)
            .Select(group => new InvoiceLineItemDto
            {
                TreatmentRecordIds = group.Select(tr => tr.Id).ToList(),
                TreatmentName = group.First().Treatment.Name,
                IsGlobal = false,
                ToothNumbers = group.Select(tr => tr.ToothRecord!.ToothNumber).OrderBy(n => n).ToList(),
                Quantity = group.Count(),
                UnitPrice = group.First().Price,
                Subtotal = group.Sum(tr => tr.Price)
            }).ToList();

        var invoice = new InvoicePreviewDto
        {
            OdontogramId = request.OdontogramId,
            GlobalTreatments = globalTreatments,
            ToothTreatments = toothTreatments,
            Subtotal = globalTreatments.Sum(t => t.Subtotal) + toothTreatments.Sum(t => t.Subtotal),
            Tax = 0,
            Total = globalTreatments.Sum(t => t.Subtotal) + toothTreatments.Sum(t => t.Subtotal)
        };

        return Result<InvoicePreviewDto>.Success(invoice);
    }
}