using AutoMapper;
using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Odontogram.DTOs;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Odontogram.Queries;

public record GetAllOdontogramTreatmentsQuery(Guid OdontogramId) 
    : IRequest<Result<IReadOnlyList<TreatmentRecordDto>>>;

public class GetAllOdontogramTreatmentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetAllOdontogramTreatmentsQuery, Result<IReadOnlyList<TreatmentRecordDto>>>
{
    public async Task<Result<IReadOnlyList<TreatmentRecordDto>>> Handle(
        GetAllOdontogramTreatmentsQuery request, 
        CancellationToken ct)
    {
        _ = await unitOfWork.Odontograms.GetByIdAsync(request.OdontogramId, ct)
            ?? throw new NotFoundException(nameof(Odontogram), request.OdontogramId);
        
        var treatmentRecords = await unitOfWork.TreatmentRecords.FindWithIncludeAsync(
            tr => tr.OdontogramId == request.OdontogramId,
            ct,
            tr => tr.Treatment,
            tr => tr.ToothRecord,
            tr => tr.Doctor
        );
        
        var dtos = treatmentRecords.Select(tr => new TreatmentRecordDto
        {
            Id = tr.Id,
            ToothRecordId = tr.ToothRecordId,
            ToothNumber = tr.ToothRecord?.ToothNumber,
            TreatmentId = tr.TreatmentId,
            TreatmentName = tr.Treatment.Name,
            TreatmentCode = tr.Treatment.Code,
            DoctorId = tr.DoctorId,
            DoctorName = tr.Doctor.FullName,
            PerformedDate = tr.PerformedDate,
            Price = tr.Price,
            Notes = tr.Notes,
            SurfacesAffected = tr.SurfacesAffected,
            IsCompleted = tr.IsCompleted,
            IsPaid = tr.IsPaid,
            IsGlobalTreatment = tr.ToothRecordId == null 
        })
        .OrderBy(tr => tr.PerformedDate)
        .ToList()
        .AsReadOnly();

        return Result<IReadOnlyList<TreatmentRecordDto>>.Success(dtos);
    }
}