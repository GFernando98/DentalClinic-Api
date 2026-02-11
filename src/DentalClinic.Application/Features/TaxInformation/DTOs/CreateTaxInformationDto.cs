using DentalClinic.Domain.Enums;

namespace DentalClinic.Application.Features.TaxInformation.DTOs;

public class CreateTaxInformationDto
{
    public string CAI { get; set; } = string.Empty;
    public InvoiceType InvoiceType { get; set; } = InvoiceType.Factura;
    public long RangeStart { get; set; }
    public long RangeEnd { get; set; }
    public DateTime AuthorizationDate { get; set; }
    public DateTime ExpirationDate { get; set; }
}