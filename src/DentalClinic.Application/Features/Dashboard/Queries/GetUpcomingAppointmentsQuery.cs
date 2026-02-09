using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Dashboard.DTOs;
using DentalClinic.Domain.Enums;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Dashboard.Queries;

public record GetUpcomingAppointmentsQuery(int Limit = 5) : IRequest<Result<IReadOnlyList<UpcomingAppointmentDto>>>;

public class GetUpcomingAppointmentsHandler : IRequestHandler<GetUpcomingAppointmentsQuery, Result<IReadOnlyList<UpcomingAppointmentDto>>>
{
    private readonly IUnitOfWork _uow;
    public GetUpcomingAppointmentsHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<IReadOnlyList<UpcomingAppointmentDto>>> Handle(GetUpcomingAppointmentsQuery request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        var appointments = await _uow.Appointments.FindAsync(
            a => a.ScheduledDate >= now
                 && (a.Status == AppointmentStatus.Scheduled || a.Status == AppointmentStatus.Confirmed), ct);

        var patients = await _uow.Patients.GetAllAsync(ct);
        var doctors = await _uow.Doctors.GetAllAsync(ct);

        var result = appointments
            .OrderBy(a => a.ScheduledDate)
            .Take(request.Limit)
            .Select(a =>
            {
                var patient = patients.FirstOrDefault(p => p.Id == a.PatientId);
                var doctor = doctors.FirstOrDefault(d => d.Id == a.DoctorId);
                return new UpcomingAppointmentDto
                {
                    Id = a.Id,
                    PatientName = patient?.FullName ?? "N/A",
                    DoctorName = doctor?.FullName ?? "N/A",
                    ScheduledDate = a.ScheduledDate,
                    ScheduledEndDate = a.ScheduledEndDate,
                    Status = (int)a.Status,
                    Reason = a.Reason
                };
            })
            .ToList();

        return Result<IReadOnlyList<UpcomingAppointmentDto>>.Success(result.AsReadOnly());
    }
}