namespace DentalClinic.Application.Features.Invoices.DTOs;

public class CreateInvoiceDto
{
    public Guid OdontogramId { get; set; }
    public List<Guid> TreatmentRecordIds { get; set; } = new();  
    public decimal? DiscountPercentage { get; set; }
    public decimal? DiscountAmount { get; set; }
    public string? Notes { get; set; }
}