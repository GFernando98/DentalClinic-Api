using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Appointments.DTOs;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Enums;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Appointments.Commands.UpdateAppointmentStatusCommand;

public record UpdateAppointmentStatusCommand(Guid Id, UpdateAppointmentStatusDto Status) : IRequest<Result<AppointmentDto>>;

public class UpdateAppointmentStatusCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateAppointmentStatusCommand, Result<AppointmentDto>>
{
    public async Task<Result<AppointmentDto>> Handle(UpdateAppointmentStatusCommand request, CancellationToken cancellationToken)
    {
        var appointment = await unitOfWork.Appointments.GetByIdAsync(request.Id, cancellationToken)
                          ?? throw new NotFoundException(nameof(Appointment), request.Id);

        appointment.Status = request.Status.Status;
        if (request.Status.Status == AppointmentStatus.Cancelled)
            appointment.CancellationReason = request.Status.CancellationReason;

        await unitOfWork.Appointments.UpdateAsync(appointment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<AppointmentDto>.Success(new AppointmentDto
        {
            Id = appointment.Id,
            PatientId = appointment.PatientId,
            DoctorId = appointment.DoctorId,
            ScheduledDate = appointment.ScheduledDate,
            ScheduledEndDate = appointment.ScheduledEndDate,
            Status = appointment.Status,
            Reason = appointment.Reason,
            CancellationReason = appointment.CancellationReason
        }, "Estado de cita actualizado.");
    }
}