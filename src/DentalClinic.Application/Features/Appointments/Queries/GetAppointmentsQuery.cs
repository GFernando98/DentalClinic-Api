using AutoMapper;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Appointments.DTOs;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Appointments.Queries;

public record GetAppointmentsQuery(DateTime? From, DateTime? To, Guid? DoctorId) : IRequest<Result<IReadOnlyList<AppointmentDto>>>;

public class GetAppointmentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetAppointmentsQuery, Result<IReadOnlyList<AppointmentDto>>>
{
    public async Task<Result<IReadOnlyList<AppointmentDto>>> Handle(GetAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var from = request.From ?? DateTime.UtcNow.Date;
        var to = request.To ?? from.AddDays(30);

        var appointments = request.DoctorId.HasValue
            ? await unitOfWork.Appointments.FindWithIncludeAsync(
                a => a.DoctorId == request.DoctorId.Value
                     && a.ScheduledDate >= from
                     && a.ScheduledDate <= to,
                cancellationToken,
                a => a.Patient,
                a => a.Doctor)
            : await unitOfWork.Appointments.FindWithIncludeAsync(
                a => a.ScheduledDate >= from && a.ScheduledDate <= to,
                cancellationToken,
                a => a.Patient,
                a => a.Doctor);

        var dtos = mapper.Map<IReadOnlyList<AppointmentDto>>(appointments);

        return Result<IReadOnlyList<AppointmentDto>>.Success(dtos);
    }
}