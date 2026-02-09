using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Appointments.DTOs;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Appointments.Queries;

public record GetPatientAppointmentsQuery(Guid PatientId) : IRequest<Result<IReadOnlyList<AppointmentDto>>>;

public class GetPatientAppointmentsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetPatientAppointmentsQuery, Result<IReadOnlyList<AppointmentDto>>>
{
    public async Task<Result<IReadOnlyList<AppointmentDto>>> Handle(GetPatientAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var appointments = await unitOfWork.Appointments.FindAsync(
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