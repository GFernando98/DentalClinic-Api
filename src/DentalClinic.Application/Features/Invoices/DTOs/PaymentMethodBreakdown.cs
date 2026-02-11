using DentalClinic.Domain.Enums;

namespace DentalClinic.Application.Features.Invoices.DTOs;

public class PaymentMethodBreakdown
{
    public PaymentMethod Method { get; set; }
    public decimal Amount { get; set; }
    public int Count { get; set; }
}