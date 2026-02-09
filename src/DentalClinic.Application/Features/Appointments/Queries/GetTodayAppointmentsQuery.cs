using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Appointments.DTOs;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Appointments.Queries;

public record GetTodayAppointmentsQuery(Guid? DoctorId) : IRequest<Result<IReadOnlyList<AppointmentDto>>>;

public class GetTodayAppointmentsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetTodayAppointmentsQuery, Result<IReadOnlyList<AppointmentDto>>>
{
    public async Task<Result<IReadOnlyList<AppointmentDto>>> Handle(GetTodayAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        IReadOnlyList<Appointment> appointments;
        if (request.DoctorId.HasValue)
        {
            appointments = await unitOfWork.Appointments.FindAsync(
                a => a.DoctorId == request.DoctorId.Value
                     && a.ScheduledDate >= today
                     && a.ScheduledDate < tomorrow, cancellationToken);
        }
        else
        {
            appointments = await unitOfWork.Appointments.FindAsync(
                a => a.ScheduledDate >= today && a.ScheduledDate < tomorrow, cancellationToken);
        }

        var dtos = appointments.Select(a => new AppointmentDto
        {
            Id = a.Id, PatientId = a.PatientId, DoctorId = a.DoctorId,
            ScheduledDate = a.ScheduledDate, ScheduledEndDate = a.ScheduledEndDate,
            Status = a.Status, Reason = a.Reason, ReminderSent = a.ReminderSent
        }).ToList().AsReadOnly();

        return Result<IReadOnlyList<AppointmentDto>>.Success(dtos);
    }
}
