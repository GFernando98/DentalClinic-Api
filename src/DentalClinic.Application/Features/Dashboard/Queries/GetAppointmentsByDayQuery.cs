using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Dashboard.DTOs;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Dashboard.Queries;

public record GetAppointmentsByDayQuery : IRequest<Result<IReadOnlyList<AppointmentsByDayDto>>>;

public class GetAppointmentsByDayHandler : IRequestHandler<GetAppointmentsByDayQuery, Result<IReadOnlyList<AppointmentsByDayDto>>>
{
    private readonly IUnitOfWork _uow;
    public GetAppointmentsByDayHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<IReadOnlyList<AppointmentsByDayDto>>> Handle(GetAppointmentsByDayQuery request, CancellationToken ct)
    {
        var today = DateTime.UtcNow.Date;
        var startDate = today.AddDays(-6);
        var dayLabels = new[] { "Dom", "Lun", "Mar", "Mié", "Jue", "Vie", "Sáb" };

        var appointments = await _uow.Appointments.FindAsync(
            a => a.ScheduledDate >= startDate && a.ScheduledDate < today.AddDays(1), ct);

        var result = new List<AppointmentsByDayDto>();
        for (int i = 0; i < 7; i++)
        {
            var day = startDate.AddDays(i);
            var nextDay = day.AddDays(1);
            result.Add(new AppointmentsByDayDto
            {
                Day = day.ToString("yyyy-MM-dd"),
                DayLabel = dayLabels[(int)day.DayOfWeek],
                Count = appointments.Count(a => a.ScheduledDate >= day && a.ScheduledDate < nextDay)
            });
        }

        return Result<IReadOnlyList<AppointmentsByDayDto>>.Success(result.AsReadOnly());
    }
}