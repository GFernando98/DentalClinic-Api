namespace DentalClinic.Application.Features.Invoices.DTOs;

public class InvoiceLineItemDto
{
    public Guid? TreatmentRecordId { get; set; }  
    public List<Guid>? TreatmentRecordIds { get; set; }
    public string TreatmentName { get; set; } = string.Empty;
    public string TreatmentCode { get; set; } = string.Empty;
    public bool IsGlobal { get; set; }
    public List<int>? ToothNumbers { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
}