namespace DentalClinic.Application.Features.Invoices.DTOs;

public class InvoiceLineDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? ToothNumbers { get; set; } 
    public bool IsGlobalTreatment { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
}