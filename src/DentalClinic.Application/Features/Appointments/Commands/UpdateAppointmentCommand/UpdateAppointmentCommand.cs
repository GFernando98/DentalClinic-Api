using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Interfaces;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Appointments.DTOs;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Enums;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Appointments.Commands.UpdateAppointmentCommand;

public record UpdateAppointmentCommand(Guid Id, CreateAppointmentDto Appointment) : IRequest<Result<AppointmentDto>>;

public class UpdateAppointmentCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<UpdateAppointmentCommand, Result<AppointmentDto>>
{
    public async Task<Result<AppointmentDto>> Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await unitOfWork.Appointments.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Appointment), request.Id);

        var dto = request.Appointment;

        // Check conflicts if date/doctor changed
        if (appointment.ScheduledDate != dto.ScheduledDate || appointment.DoctorId != dto.DoctorId)
        {
            var hasConflict = await unitOfWork.Appointments.ExistsAsync(
                a => a.Id != request.Id
                     && a.DoctorId == dto.DoctorId
                     && a.Status != AppointmentStatus.Cancelled
                     && a.ScheduledDate < dto.ScheduledEndDate
                     && a.ScheduledEndDate > dto.ScheduledDate, cancellationToken);

            if (hasConflict)
                return Result<AppointmentDto>.Failure("El doctor ya tiene una cita en ese horario.");
        }

        var patient = await unitOfWork.Patients.GetByIdAsync(dto.PatientId, cancellationToken)
            ?? throw new NotFoundException(nameof(Patient), dto.PatientId);
        var doctor = await unitOfWork.Doctors.GetByIdAsync(dto.DoctorId, cancellationToken)
            ?? throw new NotFoundException(nameof(Doctor), dto.DoctorId);

        appointment.PatientId = dto.PatientId;
        appointment.DoctorId = dto.DoctorId;
        appointment.ScheduledDate = dto.ScheduledDate;
        appointment.ScheduledEndDate = dto.ScheduledEndDate;
        appointment.Reason = dto.Reason;
        appointment.Notes = dto.Notes;
        appointment.LastModifiedBy = currentUser.UserId;

        await unitOfWork.Appointments.UpdateAsync(appointment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<AppointmentDto>.Success(new AppointmentDto
        {
            Id = appointment.Id,
            PatientId = appointment.PatientId,
            PatientName = patient.FullName,
            DoctorId = appointment.DoctorId,
            DoctorName = doctor.FullName,
            ScheduledDate = appointment.ScheduledDate,
            ScheduledEndDate = appointment.ScheduledEndDate,
            Status = appointment.Status,
            Reason = appointment.Reason,
            Notes = appointment.Notes
        }, "Cita actualizada.");
    }
}