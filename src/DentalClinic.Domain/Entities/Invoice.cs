using DentalClinic.Domain.Common;
using DentalClinic.Domain.Enums;

namespace DentalClinic.Domain.Entities;

/// <summary>
/// Factura generada
/// </summary>
public class Invoice : BaseAuditableEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;  
    
    public Guid? TaxInformationId { get; set; }  
    public TaxInformation? TaxInformation { get; set; }
    
    public InvoiceType InvoiceType { get; set; } = InvoiceType.Factura;
    
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;
    
    public Guid OdontogramId { get; set; }
    public Odontogram Odontogram { get; set; } = null!;
    
    public DateTime InvoiceDate { get; set; }
    public DateTime? DueDate { get; set; }
    
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    
    public string? Notes { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Pending;
    
    // Propiedades calculadas
    public decimal AmountPaid => Payments.Sum(p => p.Amount);
    public decimal Balance => Total - AmountPaid;
    public bool IsPaid => Status == InvoiceStatus.Paid;
    
    public ICollection<InvoiceLineItem> LineItems { get; set; } = new List<InvoiceLineItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}