using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Invoices.DTOs;
using DentalClinic.Domain.Enums;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Invoices.Queries;

public record GetRevenueReportQuery(DateTime? StartDate, DateTime? EndDate)
    : IRequest<Result<RevenueReportDto>>;

public class GetRevenueReportQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetRevenueReportQuery, Result<RevenueReportDto>>
{
    public async Task<Result<RevenueReportDto>> Handle(GetRevenueReportQuery request, CancellationToken ct)
    {
        var startDate = request.StartDate ?? DateTime.UtcNow.Date.AddMonths(-1);
        var endDate = request.EndDate ?? DateTime.UtcNow.Date;

        var invoices = await unitOfWork.Invoices.FindWithIncludeAsync(
            i => i.InvoiceDate >= startDate && i.InvoiceDate <= endDate,
            ct,
            i => i.Payments
        );

        var payments = invoices.SelectMany(i => i.Payments).ToList();

        var report = new RevenueReportDto
        {
            StartDate = startDate,
            EndDate = endDate,
            TotalInvoices = invoices.Count,
            TotalRevenue = invoices.Sum(i => i.Total),
            TotalPaid = invoices.Where(i => i.Status == InvoiceStatus.Paid).Sum(i => i.Total),
            TotalPending = invoices
                .Where(i => i.Status == InvoiceStatus.Pending || i.Status == InvoiceStatus.PartiallyPaid)
                .Sum(i => i.Balance),
            TotalCancelled = invoices.Where(i => i.Status == InvoiceStatus.Cancelled).Sum(i => i.Total),

            PaymentMethodBreakdown = payments
                .GroupBy(p => p.PaymentMethod)
                .Select(g => new PaymentMethodBreakdown
                {
                    Method = g.Key,
                    Amount = g.Sum(p => p.Amount),
                    Count = g.Count()
                })
                .ToList(),

            DailyRevenue = invoices
                .GroupBy(i => i.InvoiceDate.Date)
                .Select(g => new DailyRevenue
                {
                    Date = g.Key,
                    Amount = g.Sum(i => i.Total),
                    InvoiceCount = g.Count()
                })
                .OrderBy(d => d.Date)
                .ToList()
        };

        return Result<RevenueReportDto>.Success(report);
    }
}