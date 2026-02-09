namespace DentalClinic.Application.Features.Invoices.DTOs;

public class InvoicePreviewDto
{
    public Guid OdontogramId { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public List<InvoiceLineItemDto> GlobalTreatments { get; set; } = new();
    public List<InvoiceLineItemDto> ToothTreatments { get; set; } = new();
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
}