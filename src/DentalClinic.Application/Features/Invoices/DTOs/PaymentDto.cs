using DentalClinic.Domain.Enums;

namespace DentalClinic.Application.Features.Invoices.DTOs;

public class PaymentDto
{
    public Guid Id { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
}