using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Dashboard.DTOs;
using DentalClinic.Domain.Enums;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Dashboard.Queries;

public record GetDashboardStatsQuery : IRequest<Result<DashboardStatsDto>>;

public class GetDashboardStatsHandler : IRequestHandler<GetDashboardStatsQuery, Result<DashboardStatsDto>>
{
    private readonly IUnitOfWork _uow;
    public GetDashboardStatsHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<DashboardStatsDto>> Handle(GetDashboardStatsQuery request, CancellationToken ct)
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);
        var monthStart = new DateTime(today.Year, today.Month, 1);

        var todayAppts = await _uow.Appointments.CountAsync(
            a => a.ScheduledDate >= today && a.ScheduledDate < tomorrow, ct);

        var totalPatients = await _uow.Patients.CountAsync(_ => true, ct);

        var pending = await _uow.Appointments.CountAsync(
            a => a.Status == AppointmentStatus.Scheduled || a.Status == AppointmentStatus.Confirmed, ct);

        var monthTreatments = await _uow.TreatmentRecords.CountAsync(
            tr => tr.PerformedDate >= monthStart && tr.PerformedDate < tomorrow, ct);

        return Result<DashboardStatsDto>.Success(new DashboardStatsDto
        {
            TodayAppointments = todayAppts,
            TotalPatients = totalPatients,
            PendingAppointments = pending,
            MonthTreatments = monthTreatments
        });
    }
}