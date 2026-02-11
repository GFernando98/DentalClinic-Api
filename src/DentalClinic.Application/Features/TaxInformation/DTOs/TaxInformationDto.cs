using DentalClinic.Domain.Enums;

namespace DentalClinic.Application.Features.TaxInformation.DTOs;

public class TaxInformationDto
{
    public Guid Id { get; set; }
    public string CAI { get; set; } = string.Empty;
    public InvoiceType InvoiceType { get; set; }
    public long RangeStart { get; set; }
    public long RangeEnd { get; set; }
    public long CurrentNumber { get; set; }
    public DateTime AuthorizationDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsExpired { get; set; }
    public bool IsExhausted { get; set; }
    public bool CanGenerateInvoice { get; set; }
    public long RemainingInvoices { get; set; }
}