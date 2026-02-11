namespace DentalClinic.Application.Features.Invoices.DTOs;

public class DailyRevenue
{
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public int InvoiceCount { get; set; }
}