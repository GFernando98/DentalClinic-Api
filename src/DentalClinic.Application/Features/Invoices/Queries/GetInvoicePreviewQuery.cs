using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Invoices.DTOs;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Invoices.Queries;

public record GetInvoicePreviewQuery(Guid OdontogramId) : IRequest<Result<InvoicePreviewDto>>;

public class GetInvoicePreviewQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetInvoicePreviewQuery, Result<InvoicePreviewDto>>
{
    public async Task<Result<InvoicePreviewDto>> Handle(GetInvoicePreviewQuery request, CancellationToken ct)
    {
        // 1. Verificar que el odontograma existe
        var odontogram = await unitOfWork.Odontograms.GetByIdAsync(request.OdontogramId, ct)
            ?? throw new NotFoundException(nameof(Odontogram), request.OdontogramId);

        // 2. Obtener paciente
        var patient = await unitOfWork.Patients.GetByIdAsync(odontogram.PatientId, ct)
            ?? throw new NotFoundException(nameof(Patient), odontogram.PatientId);

        // 3. Obtener TODOS los TreatmentRecords NO pagados del odontograma
        var treatmentRecords = await unitOfWork.TreatmentRecords.FindWithIncludeAsync(
            tr => tr.OdontogramId == request.OdontogramId && !tr.IsPaid,
            ct,
            tr => tr.Treatment,
            tr => tr.ToothRecord
        );

        if (!treatmentRecords.Any())
        {
            return Result<InvoicePreviewDto>.Failure("No hay tratamientos pendientes de pago para este odontograma.");
        }

        // 4. Separar tratamientos globales de los por diente
        var globalTreatments = treatmentRecords
            .Where(tr => tr.ToothRecordId == null)
            .Select(tr => new InvoiceLineItemDto
            {
                TreatmentRecordId = tr.Id,
                TreatmentName = tr.Treatment.Name,
                TreatmentCode = tr.Treatment.Code,
                IsGlobal = true,
                ToothNumbers = null,
                Quantity = 1,
                UnitPrice = tr.Price,
                Subtotal = tr.Price
            }).ToList();

        // 5. Agrupar tratamientos por diente (mismo tratamiento en múltiples dientes)
        var toothTreatments = treatmentRecords
            .Where(tr => tr.ToothRecordId != null)
            .GroupBy(tr => tr.TreatmentId)
            .Select(group => new InvoiceLineItemDto
            {
                TreatmentRecordIds = group.Select(tr => tr.Id).ToList(),
                TreatmentName = group.First().Treatment.Name,
                TreatmentCode = group.First().Treatment.Code,
                IsGlobal = false,
                ToothNumbers = group.Select(tr => tr.ToothRecord!.ToothNumber).OrderBy(n => n).ToList(),
                Quantity = group.Count(),
                UnitPrice = group.First().Price,
                Subtotal = group.Sum(tr => tr.Price)
            }).ToList();

        // 6. Calcular totales
        var subtotal = globalTreatments.Sum(t => t.Subtotal) + toothTreatments.Sum(t => t.Subtotal);
        var tax = 0m; // Puedes agregar lógica de impuestos aquí si es necesario

        var invoice = new InvoicePreviewDto
        {
            OdontogramId = request.OdontogramId,
            PatientId = patient.Id,
            PatientName = patient.FullName,
            GlobalTreatments = globalTreatments,
            ToothTreatments = toothTreatments,
            Subtotal = subtotal,
            Tax = tax,
            Discount = 0,
            Total = subtotal + tax
        };

        return Result<InvoicePreviewDto>.Success(invoice);
    }
}