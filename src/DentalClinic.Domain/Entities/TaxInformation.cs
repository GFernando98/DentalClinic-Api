using DentalClinic.Domain.Common;
using DentalClinic.Domain.Enums;

namespace DentalClinic.Domain.Entities;

// src/DentalClinic.Domain/Entities/TaxInformation.cs
public class TaxInformation : BaseAuditableEntity
{
    public string CAI { get; set; } = string.Empty;
    public InvoiceType InvoiceType { get; set; } = InvoiceType.Factura;
    public string RangeStart { get; set; } = string.Empty;
    public string RangeEnd { get; set; } = string.Empty;
    public string Branch { get; set; } = string.Empty;
    public string PointEmission { get; set; } = string.Empty;
    public string CurrentNumber { get; set; } = string.Empty;
    public DateTime AuthorizationDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool IsActive { get; set; } = true;
    public bool HasBeenUsed { get; set; } = false;
    
    public bool IsExpired => DateTime.UtcNow > ExpirationDate;
    
    public bool IsExhausted
    {
        get
        {
            if (int.TryParse(CurrentNumber, out var current) && 
                int.TryParse(RangeEnd, out var end))
            {
                return current >= end;
            }
            return false;
        }
    }
    
    public bool CanGenerateInvoice => IsActive && !IsExpired && !IsExhausted;
    
    public bool IsNearExhaustion
    {
        get
        {
            if (int.TryParse(CurrentNumber, out var current) && 
                int.TryParse(RangeEnd, out var end) &&
                int.TryParse(RangeStart, out var start))
            {
                var totalRange = end - start;
                var remaining = end - current;
                var percentageRemaining = (double)remaining / totalRange * 100;
                return percentageRemaining <= 10; // Menos del 10%
            }
            return false;
        }
    }
    
    public long RemainingInvoices
    {
        get
        {
            if (int.TryParse(RangeEnd, out var end) && 
                int.TryParse(CurrentNumber, out var current))
            {
                return end - current;
            }
            return 0;
        }
    }
    
    public string GetFullInvoiceNumber()
    {
        return $"{Branch}-{PointEmission}-{CurrentNumber}";
    }
}