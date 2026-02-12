using DentalClinic.Domain.Enums;

namespace DentalClinic.Application.Features.TaxInformation.DTOs;

public class CreateTaxInformationDto
{
    public string CAI { get; set; } = string.Empty;
    public InvoiceType InvoiceType { get; set; } = InvoiceType.Factura;
    public string RangeStart { get; set; }
    public string RangeEnd { get; set; }
    public string Branch { get; set; }
    public string PointEmission { get; set; }
    public DateTime AuthorizationDate { get; set; }
    public DateTime ExpirationDate { get; set; }
}