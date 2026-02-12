// src/DentalClinic.Application/Features/Invoices/Commands/CreateInvoiceCommand.cs

using System.Text.Json;
using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Interfaces;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Invoices.DTOs;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Enums;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Invoices.Commands.CreateInvoiceCommand;

public record CreateInvoiceCommand(CreateInvoiceDto Invoice) : IRequest<Result<InvoiceDto>>;

public class CreateInvoiceCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<CreateInvoiceCommand, Result<InvoiceDto>>
{
    public async Task<Result<InvoiceDto>> Handle(CreateInvoiceCommand request, CancellationToken ct)
    {
        var dto = request.Invoice;

        // 1. Verificar odontograma
        var odontogram = await unitOfWork.Odontograms.FindWithIncludeAsync(
            o => o.Id == dto.OdontogramId,
            ct,
            o => o.Patient
        );

        var odonto = odontogram.FirstOrDefault()
            ?? throw new NotFoundException(nameof(Odontogram), dto.OdontogramId);

        // 2. Obtener TreatmentRecords
        var treatmentRecords = await unitOfWork.TreatmentRecords.FindWithIncludeAsync(
            tr => dto.TreatmentRecordIds.Contains(tr.Id) && !tr.IsPaid,
            ct,
            tr => tr.Treatment,
            tr => tr.ToothRecord
        );

        if (!treatmentRecords.Any())
        {
            return Result<InvoiceDto>.Failure("No se encontraron tratamientos pendientes de pago.");
        }

        // 3. Obtener o generar número de factura
        var (invoiceNumber, taxInfo) = await GetNextInvoiceNumberAsync(ct);

        if (invoiceNumber == null)
        {
            return Result<InvoiceDto>.Failure(
                "No hay CAI disponible y no se puede generar factura sin autorización fiscal.");
        }

        // 4. Calcular totales
        var subtotal = treatmentRecords.Sum(tr => tr.Price);
        var discount = dto.DiscountAmount ?? (subtotal * (dto.DiscountPercentage ?? 0) / 100);
        var tax = 0m; // Configurar impuestos según legislación
        var total = subtotal - discount + tax;

        // 5. Crear factura
        var invoice = new Invoice
        {
            InvoiceNumber = invoiceNumber,
            TaxInformationId = taxInfo?.Id,
            InvoiceType = InvoiceType.Factura,
            PatientId = odonto.PatientId,
            OdontogramId = dto.OdontogramId,
            InvoiceDate = DateTime.UtcNow,
            Subtotal = subtotal,
            Tax = tax,
            Discount = discount,
            Total = total,
            Notes = dto.Notes,
            Status = InvoiceStatus.Pending,
            CreatedBy = currentUser.UserId
        };

        // 6. Crear líneas de factura - Tratamientos globales
        var globalTreatments = treatmentRecords
            .Where(tr => tr.ToothRecordId == null)
            .ToList();

        foreach (var tr in globalTreatments)
        {
            invoice.LineItems.Add(new InvoiceLineItem
            {
                TreatmentId = tr.TreatmentId,
                Description = tr.Treatment.Name,
                IsGlobalTreatment = true,
                ToothNumbers = null,
                Quantity = 1,
                UnitPrice = tr.Price,
                Subtotal = tr.Price,
                TreatmentRecordIds = JsonSerializer.Serialize(new[] { tr.Id })
            });
        }

        // 7. Crear líneas de factura - Tratamientos por diente (agrupados)
        var toothTreatments = treatmentRecords
            .Where(tr => tr.ToothRecordId != null)
            .GroupBy(tr => tr.TreatmentId)
            .ToList();

        foreach (var group in toothTreatments)
        {
            var teeth = group.Select(tr => tr.ToothRecord!.ToothNumber).OrderBy(n => n).ToList();
            var ids = group.Select(tr => tr.Id).ToList();

            invoice.LineItems.Add(new InvoiceLineItem
            {
                TreatmentId = group.Key,
                Description = group.First().Treatment.Name,
                IsGlobalTreatment = false,
                ToothNumbers = string.Join(", ", teeth),
                Quantity = group.Count(),
                UnitPrice = group.First().Price,
                Subtotal = group.Sum(tr => tr.Price),
                TreatmentRecordIds = JsonSerializer.Serialize(ids)
            });
        }

        // 8. Guardar factura
        await unitOfWork.Invoices.AddAsync(invoice, ct);

        // 9. Actualizar CAI si existe
        if (taxInfo != null)
        {
            if (int.TryParse(taxInfo.CurrentNumber, out var current))
            {
                current++;
                var length = taxInfo.CurrentNumber.Length;
                taxInfo.CurrentNumber = current.ToString().PadLeft(length, '0');
                
                // Marcar como usado
                taxInfo.HasBeenUsed = true;
                
                await unitOfWork.TaxInformation.UpdateAsync(taxInfo, ct);
            }
        }

        await unitOfWork.SaveChangesAsync(ct);

        // 10. Retornar DTO
        return Result<InvoiceDto>.Success(new InvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            InvoiceType = invoice.InvoiceType,
            PatientId = invoice.PatientId,
            PatientName = odonto.Patient.FullName,
            OdontogramId = invoice.OdontogramId,
            InvoiceDate = invoice.InvoiceDate,
            DueDate = invoice.DueDate,
            Subtotal = invoice.Subtotal,
            Tax = invoice.Tax,
            Discount = invoice.Discount,
            Total = invoice.Total,
            AmountPaid = 0,
            Balance = invoice.Total,
            Notes = invoice.Notes,
            Status = invoice.Status,
            CAI = taxInfo?.CAI,
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
            Payments = new()
        }, "Factura creada exitosamente.");
    }

    private async Task<(string? invoiceNumber, Domain.Entities.TaxInformation? taxInfo)> GetNextInvoiceNumberAsync(
        CancellationToken ct)
    {
        // 1. Buscar CAI activo
        var activeTaxInfo = (await unitOfWork.TaxInformation.FindAsync(
            t => t.IsActive && t.InvoiceType == InvoiceType.Factura,
            ct
        ))
        .Where(t => t.CanGenerateInvoice)
        .FirstOrDefault();

        // 2. Si el CAI activo está por agotarse (≤10 facturas restantes), auto-switch
        if (activeTaxInfo != null && activeTaxInfo.RemainingInvoices <= 10)
        {
            // Buscar siguiente CAI disponible
            var nextCAI = (await unitOfWork.TaxInformation.FindAsync(
                t => !t.IsActive && 
                     !t.HasBeenUsed && 
                     t.InvoiceType == InvoiceType.Factura,
                ct
            ))
            .Where(t => !t.IsExpired && !t.IsExhausted)
            .OrderBy(t => t.ExpirationDate)
            .FirstOrDefault();

            if (nextCAI != null)
            {
                // Desactivar el actual
                activeTaxInfo.IsActive = false;
                await unitOfWork.TaxInformation.UpdateAsync(activeTaxInfo, ct);

                // Activar el siguiente
                nextCAI.IsActive = true;
                await unitOfWork.TaxInformation.UpdateAsync(nextCAI, ct);

                // Usar el nuevo CAI
                activeTaxInfo = nextCAI;
            }
        }

        // 3. Si hay CAI activo, usarlo
        if (activeTaxInfo != null)
        {
            var fullNumber = activeTaxInfo.GetFullInvoiceNumber();
            return (fullNumber, activeTaxInfo);
        }

        // 4. Si NO hay CAI disponible, autogenerar
        var lastInvoice = (await unitOfWork.Invoices.FindAsync(
            i => i.TaxInformationId == null,
            ct
        ))
        .OrderByDescending(i => i.CreatedAt)
        .FirstOrDefault();

        var nextNumber = 1;
        if (lastInvoice != null && lastInvoice.InvoiceNumber.StartsWith("AUTO-"))
        {
            var parts = lastInvoice.InvoiceNumber.Split('-');
            if (parts.Length == 2 && int.TryParse(parts[1], out var num))
            {
                nextNumber = num + 1;
            }
        }

        return ($"AUTO-{nextNumber:D8}", null);
    }
}