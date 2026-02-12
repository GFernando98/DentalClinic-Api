using DentalClinic.Domain.Enums;

namespace DentalClinic.Application.Features.TaxInformation.DTOs;

public class TaxInformationDto
{
    public Guid Id { get; set; }
    public string CAI { get; set; } = string.Empty;
    public InvoiceType InvoiceType { get; set; }
    public string RangeStart { get; set; }
    public string RangeEnd { get; set; }
    public string Branch { get; set; }
    public string PointEmission { get; set; }
    public string CurrentNumber { get; set; }
    public DateTime AuthorizationDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsExpired { get; set; }
    public bool IsExhausted { get; set; }
    public bool CanGenerateInvoice { get; set; }
    public long RemainingInvoices { get; set; }
    public bool HasBeenUsed { get; set; } 
}