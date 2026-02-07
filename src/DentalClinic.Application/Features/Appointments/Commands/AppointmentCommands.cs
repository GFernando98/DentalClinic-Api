using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Interfaces;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Appointments.DTOs;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Enums;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Appointments.Commands;

// ─── CREATE ───────────────────────────────────────────────────────────
public record CreateAppointmentCommand(CreateAppointmentDto Appointment) : IRequest<Result<AppointmentDto>>;

public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, Result<AppointmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public CreateAppointmentCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<AppointmentDto>> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Appointment;

        // Validate patient
        var patient = await _unitOfWork.Patients.GetByIdAsync(dto.PatientId, cancellationToken)
            ?? throw new NotFoundException(nameof(Patient), dto.PatientId);

        // Validate doctor
        var doctor = await _unitOfWork.Doctors.GetByIdAsync(dto.DoctorId, cancellationToken)
            ?? throw new NotFoundException(nameof(Doctor), dto.DoctorId);

        // Check for overlapping appointments
        var hasConflict = await _unitOfWork.Appointments.ExistsAsync(
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
            CreatedBy = _currentUser.UserId
        };

        await _unitOfWork.Appointments.AddAsync(appointment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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

// ─── UPDATE ───────────────────────────────────────────────────────────
public record UpdateAppointmentCommand(Guid Id, CreateAppointmentDto Appointment) : IRequest<Result<AppointmentDto>>;

public class UpdateAppointmentCommandHandler : IRequestHandler<UpdateAppointmentCommand, Result<AppointmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public UpdateAppointmentCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<AppointmentDto>> Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Appointment), request.Id);

        var dto = request.Appointment;

        // Check conflicts if date/doctor changed
        if (appointment.ScheduledDate != dto.ScheduledDate || appointment.DoctorId != dto.DoctorId)
        {
            var hasConflict = await _unitOfWork.Appointments.ExistsAsync(
                a => a.Id != request.Id
                     && a.DoctorId == dto.DoctorId
                     && a.Status != AppointmentStatus.Cancelled
                     && a.ScheduledDate < dto.ScheduledEndDate
                     && a.ScheduledEndDate > dto.ScheduledDate, cancellationToken);

            if (hasConflict)
                return Result<AppointmentDto>.Failure("El doctor ya tiene una cita en ese horario.");
        }

        var patient = await _unitOfWork.Patients.GetByIdAsync(dto.PatientId, cancellationToken)
            ?? throw new NotFoundException(nameof(Patient), dto.PatientId);
        var doctor = await _unitOfWork.Doctors.GetByIdAsync(dto.DoctorId, cancellationToken)
            ?? throw new NotFoundException(nameof(Doctor), dto.DoctorId);

        appointment.PatientId = dto.PatientId;
        appointment.DoctorId = dto.DoctorId;
        appointment.ScheduledDate = dto.ScheduledDate;
        appointment.ScheduledEndDate = dto.ScheduledEndDate;
        appointment.Reason = dto.Reason;
        appointment.Notes = dto.Notes;
        appointment.LastModifiedBy = _currentUser.UserId;

        await _unitOfWork.Appointments.UpdateAsync(appointment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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

// ─── UPDATE STATUS ────────────────────────────────────────────────────
public record UpdateAppointmentStatusCommand(Guid Id, UpdateAppointmentStatusDto Status) : IRequest<Result<AppointmentDto>>;

public class UpdateAppointmentStatusCommandHandler : IRequestHandler<UpdateAppointmentStatusCommand, Result<AppointmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAppointmentStatusCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AppointmentDto>> Handle(UpdateAppointmentStatusCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Appointment), request.Id);

        appointment.Status = request.Status.Status;
        if (request.Status.Status == AppointmentStatus.Cancelled)
            appointment.CancellationReason = request.Status.CancellationReason;

        await _unitOfWork.Appointments.UpdateAsync(appointment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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

// ─── DELETE (Soft) ────────────────────────────────────────────────────
public record DeleteAppointmentCommand(Guid Id) : IRequest<Result<bool>>;

public class DeleteAppointmentCommandHandler : IRequestHandler<DeleteAppointmentCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public DeleteAppointmentCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<bool>> Handle(DeleteAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Appointment), request.Id);

        appointment.IsDeleted = true;
        appointment.DeletedAt = DateTime.UtcNow;
        appointment.DeletedBy = _currentUser.UserId;

        await _unitOfWork.Appointments.UpdateAsync(appointment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "Cita eliminada.");
    }
}
