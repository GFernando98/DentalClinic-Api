using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Odontogram.DTOs;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Odontogram.Queries;

// ─── GET BY PATIENT ───────────────────────────────────────────────────
public record GetPatientOdontogramsQuery(Guid PatientId) : IRequest<Result<IReadOnlyList<OdontogramDto>>>;

public class GetPatientOdontogramsQueryHandler : IRequestHandler<GetPatientOdontogramsQuery, Result<IReadOnlyList<OdontogramDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPatientOdontogramsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<OdontogramDto>>> Handle(GetPatientOdontogramsQuery request, CancellationToken cancellationToken)
    {
        var odontograms = await _unitOfWork.Odontograms.FindAsync(
            o => o.PatientId == request.PatientId, cancellationToken);

        var dtos = odontograms.Select(o => new OdontogramDto
        {
            Id = o.Id,
            PatientId = o.PatientId,
            ExaminationDate = o.ExaminationDate,
            Notes = o.Notes,
            DoctorId = o.DoctorId
        }).ToList().AsReadOnly();

        return Result<IReadOnlyList<OdontogramDto>>.Success(dtos);
    }
}

// ─── GET BY ID ────────────────────────────────────────────────────────
public record GetOdontogramByIdQuery(Guid Id) : IRequest<Result<OdontogramDto>>;

public class GetOdontogramByIdQueryHandler : IRequestHandler<GetOdontogramByIdQuery, Result<OdontogramDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetOdontogramByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<OdontogramDto>> Handle(GetOdontogramByIdQuery request, CancellationToken cancellationToken)
    {
        var odontogram = await _unitOfWork.Odontograms.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Odontograma", request.Id);

        // Get teeth
        var teeth = await _unitOfWork.ToothRecords.FindAsync(
            t => t.OdontogramId == request.Id, cancellationToken);

        var teethDtos = new List<ToothRecordDto>();
        foreach (var tooth in teeth)
        {
            var surfaces = await _unitOfWork.ToothSurfaceRecords.FindAsync(
                s => s.ToothRecordId == tooth.Id, cancellationToken);

            teethDtos.Add(new ToothRecordDto
            {
                Id = tooth.Id,
                ToothNumber = tooth.ToothNumber,
                ToothType = tooth.ToothType,
                Condition = tooth.Condition,
                IsPresent = tooth.IsPresent,
                Notes = tooth.Notes,
                Surfaces = surfaces.Select(s => new ToothSurfaceDto
                {
                    Id = s.Id, Surface = s.Surface,
                    Condition = s.Condition, Notes = s.Notes
                }).ToList()
            });
        }

        return Result<OdontogramDto>.Success(new OdontogramDto
        {
            Id = odontogram.Id,
            PatientId = odontogram.PatientId,
            ExaminationDate = odontogram.ExaminationDate,
            Notes = odontogram.Notes,
            DoctorId = odontogram.DoctorId,
            TeethRecords = teethDtos.OrderBy(t => t.ToothNumber).ToList()
        });
    }
}

// ─── GET TOOTH TREATMENTS ─────────────────────────────────────────────
public record GetToothTreatmentsQuery(Guid ToothRecordId) : IRequest<Result<IReadOnlyList<TreatmentRecordDto>>>;

public class GetToothTreatmentsQueryHandler : IRequestHandler<GetToothTreatmentsQuery, Result<IReadOnlyList<TreatmentRecordDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetToothTreatmentsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<TreatmentRecordDto>>> Handle(GetToothTreatmentsQuery request, CancellationToken cancellationToken)
    {
        var records = await _unitOfWork.TreatmentRecords.FindAsync(
            tr => tr.ToothRecordId == request.ToothRecordId, cancellationToken);

        var dtos = new List<TreatmentRecordDto>();
        foreach (var r in records)
        {
            var treatment = await _unitOfWork.Treatments.GetByIdAsync(r.TreatmentId, cancellationToken);
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(r.DoctorId, cancellationToken);

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
