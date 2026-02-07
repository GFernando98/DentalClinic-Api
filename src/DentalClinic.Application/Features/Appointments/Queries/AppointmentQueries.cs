using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Appointments.DTOs;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Appointments.Queries;

// ─── GET ALL (with filters) ──────────────────────────────────────────
public record GetAppointmentsQuery(DateTime? From, DateTime? To, Guid? DoctorId) : IRequest<Result<IReadOnlyList<AppointmentDto>>>;

public class GetAppointmentsQueryHandler : IRequestHandler<GetAppointmentsQuery, Result<IReadOnlyList<AppointmentDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAppointmentsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<AppointmentDto>>> Handle(GetAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var from = request.From ?? DateTime.UtcNow.Date;
        var to = request.To ?? from.AddDays(30);

        IReadOnlyList<Appointment> appointments;

        if (request.DoctorId.HasValue)
        {
            appointments = await _unitOfWork.Appointments.FindAsync(
                a => a.DoctorId == request.DoctorId.Value
                     && a.ScheduledDate >= from
                     && a.ScheduledDate <= to, cancellationToken);
        }
        else
        {
            appointments = await _unitOfWork.Appointments.FindAsync(
                a => a.ScheduledDate >= from && a.ScheduledDate <= to, cancellationToken);
        }

        var dtos = appointments.Select(a => new AppointmentDto
        {
            Id = a.Id,
            PatientId = a.PatientId,
            DoctorId = a.DoctorId,
            ScheduledDate = a.ScheduledDate,
            ScheduledEndDate = a.ScheduledEndDate,
            Status = a.Status,
            Reason = a.Reason,
            Notes = a.Notes,
            ReminderSent = a.ReminderSent
        }).ToList().AsReadOnly();

        return Result<IReadOnlyList<AppointmentDto>>.Success(dtos);
    }
}

// ─── GET BY ID ────────────────────────────────────────────────────────
public record GetAppointmentByIdQuery(Guid Id) : IRequest<Result<AppointmentDto>>;

public class GetAppointmentByIdQueryHandler : IRequestHandler<GetAppointmentByIdQuery, Result<AppointmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAppointmentByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AppointmentDto>> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
    {
        var a = await _unitOfWork.Appointments.GetByIdAsync(request.Id, cancellationToken);
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

// ─── GET TODAY ─────────────────────────────────────────────────────────
public record GetTodayAppointmentsQuery(Guid? DoctorId) : IRequest<Result<IReadOnlyList<AppointmentDto>>>;

public class GetTodayAppointmentsQueryHandler : IRequestHandler<GetTodayAppointmentsQuery, Result<IReadOnlyList<AppointmentDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTodayAppointmentsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<AppointmentDto>>> Handle(GetTodayAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        IReadOnlyList<Appointment> appointments;
        if (request.DoctorId.HasValue)
        {
            appointments = await _unitOfWork.Appointments.FindAsync(
                a => a.DoctorId == request.DoctorId.Value
                     && a.ScheduledDate >= today
                     && a.ScheduledDate < tomorrow, cancellationToken);
        }
        else
        {
            appointments = await _unitOfWork.Appointments.FindAsync(
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

// ─── GET BY PATIENT ───────────────────────────────────────────────────
public record GetPatientAppointmentsQuery(Guid PatientId) : IRequest<Result<IReadOnlyList<AppointmentDto>>>;

public class GetPatientAppointmentsQueryHandler : IRequestHandler<GetPatientAppointmentsQuery, Result<IReadOnlyList<AppointmentDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPatientAppointmentsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<AppointmentDto>>> Handle(GetPatientAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var appointments = await _unitOfWork.Appointments.FindAsync(
            a => a.PatientId == request.PatientId, cancellationToken);

        var dtos = appointments.Select(a => new AppointmentDto
        {
            Id = a.Id, PatientId = a.PatientId, DoctorId = a.DoctorId,
            ScheduledDate = a.ScheduledDate, ScheduledEndDate = a.ScheduledEndDate,
            Status = a.Status, Reason = a.Reason, Notes = a.Notes
        }).ToList().AsReadOnly();

        return Result<IReadOnlyList<AppointmentDto>>.Success(dtos);
    }
}
