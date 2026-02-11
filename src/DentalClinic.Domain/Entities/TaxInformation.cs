using DentalClinic.Domain.Common;
using DentalClinic.Domain.Enums;

namespace DentalClinic.Domain.Entities;

public class TaxInformation : BaseAuditableEntity
{
    public string CAI { get; set; } = string.Empty;  
    public InvoiceType InvoiceType { get; set; } = InvoiceType.Factura;
    public long RangeStart { get; set; }
    public long RangeEnd { get; set; }  
    public long CurrentNumber { get; set; }
    public DateTime AuthorizationDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool IsActive { get; set; } = true;
    
    public bool IsExpired => DateTime.UtcNow > ExpirationDate;
    public bool IsExhausted => CurrentNumber >= RangeEnd;
    public bool CanGenerateInvoice => IsActive && !IsExpired && !IsExhausted;
    public long RemainingInvoices => RangeEnd - CurrentNumber;
}