using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Interfaces;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Appointments.DTOs;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Enums;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Appointments.Commands.CreateAppointmentCommand;

public record CreateAppointmentCommand(CreateAppointmentDto Appointment) : IRequest<Result<AppointmentDto>>;

public class CreateAppointmentCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<CreateAppointmentCommand, Result<AppointmentDto>>
{
    public async Task<Result<AppointmentDto>> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Appointment;

        // Validate patient
        var patient = await unitOfWork.Patients.GetByIdAsync(dto.PatientId, cancellationToken)
            ?? throw new NotFoundException(nameof(Patient), dto.PatientId);

        // Validate doctor
        var doctor = await unitOfWork.Doctors.GetByIdAsync(dto.DoctorId, cancellationToken)
            ?? throw new NotFoundException(nameof(Doctor), dto.DoctorId);

        // Check for overlapping appointments
        var hasConflict = await unitOfWork.Appointments.ExistsAsync(
            a => a.DoctorId == dto.DoctorId
                 && a.Status != AppointmentStatus.Cancelled
                 && a.ScheduledDate < dto.ScheduledEndDate
                 && a.ScheduledEndDate > dto.ScheduledDate, cancellationToken);

        if (hasConflict)
            return Result<AppointmentDto>.Failure("El doctor ya tiene una cita en ese horario.");

        var appointment = new Appointment
        {
            PatientId = dto.PatientId,
            DoctorId = dto.DoctorId,
            ScheduledDate = dto.ScheduledDate,
            ScheduledEndDate = dto.ScheduledEndDate,
            Reason = dto.Reason,
            Notes = dto.Notes,
            Status = AppointmentStatus.Scheduled,
            CreatedBy = currentUser.UserId
        };

        await unitOfWork.Appointments.AddAsync(appointment, cancellationToken);
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
        }, "Cita creada exitosamente.");
    }
}