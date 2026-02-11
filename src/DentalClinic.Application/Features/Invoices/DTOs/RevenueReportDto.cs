namespace DentalClinic.Application.Features.Invoices.DTOs;

public class RevenueReportDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalInvoices { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal TotalPending { get; set; }
    public decimal TotalCancelled { get; set; }
    public List<PaymentMethodBreakdown> PaymentMethodBreakdown { get; set; } = new();
    public List<DailyRevenue> DailyRevenue { get; set; } = new();
}