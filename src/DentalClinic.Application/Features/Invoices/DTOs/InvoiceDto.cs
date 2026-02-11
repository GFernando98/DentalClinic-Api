using DentalClinic.Domain.Enums;

namespace DentalClinic.Application.Features.Invoices.DTOs;

public class InvoiceDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public InvoiceType InvoiceType { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public Guid OdontogramId { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal Balance { get; set; }
    public string? Notes { get; set; }
    public InvoiceStatus Status { get; set; }
    public string? CAI { get; set; }
    public List<InvoiceLineDto> LineItems { get; set; } = new();
    public List<PaymentDto> Payments { get; set; } = new();
}