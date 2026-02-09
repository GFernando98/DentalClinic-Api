using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Odontogram.DTOs;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Odontogram.Queries;

public record GetToothTreatmentsQuery(Guid ToothRecordId) : IRequest<Result<IReadOnlyList<TreatmentRecordDto>>>;

public class GetToothTreatmentsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetToothTreatmentsQuery, Result<IReadOnlyList<TreatmentRecordDto>>>
{
    public async Task<Result<IReadOnlyList<TreatmentRecordDto>>> Handle(GetToothTreatmentsQuery request, CancellationToken cancellationToken)
    {
        var records = await unitOfWork.TreatmentRecords.FindAsync(
            tr => tr.ToothRecordId == request.ToothRecordId, cancellationToken);

        var dtos = new List<TreatmentRecordDto>();
        foreach (var r in records)
        {
            var treatment = await unitOfWork.Treatments.GetByIdAsync(r.TreatmentId, cancellationToken);
            var doctor = await unitOfWork.Doctors.GetByIdAsync(r.DoctorId, cancellationToken);

            dtos.Add(new TreatmentRecordDto
            {
                Id = r.Id, ToothRecordId = r.ToothRecordId,
                TreatmentId = r.TreatmentId,
                TreatmentName = treatment?.Name ?? "N/A",
                TreatmentCode = treatment?.Code,
                DoctorId = r.DoctorId,
                DoctorName = doctor?.FullName ?? "N/A",
                PerformedDate = r.PerformedDate,
                Price = r.Price, Notes = r.Notes,
                SurfacesAffected = r.SurfacesAffected,
                IsCompleted = r.IsCompleted
            });
        }

        return Result<IReadOnlyList<TreatmentRecordDto>>.Success(dtos.AsReadOnly());
    }
}