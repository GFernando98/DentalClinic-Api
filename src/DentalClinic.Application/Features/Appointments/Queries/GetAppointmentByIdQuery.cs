using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Appointments.DTOs;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Appointments.Queries;

public record GetAppointmentByIdQuery(Guid Id) : IRequest<Result<AppointmentDto>>;

public class GetAppointmentByIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetAppointmentByIdQuery, Result<AppointmentDto>>
{
    public async Task<Result<AppointmentDto>> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
    {
        var a = await unitOfWork.Appointments.GetByIdAsync(request.Id, cancellationToken);
        if (a == null)
            return Result<AppointmentDto>.Failure("Cita no encontrada.");

        return Result<AppointmentDto>.Success(new AppointmentDto
        {
            Id = a.Id, PatientId = a.PatientId, DoctorId = a.DoctorId,
            ScheduledDate = a.ScheduledDate, ScheduledEndDate = a.ScheduledEndDate,
            Status = a.Status, Reason = a.Reason, Notes = a.Notes,
            CancellationReason = a.CancellationReason, ReminderSent = a.ReminderSent
        });
    }
}
