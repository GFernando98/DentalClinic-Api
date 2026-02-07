using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Interfaces;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Odontogram.DTOs;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Enums;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Odontogram.Commands;

// ─── CREATE ODONTOGRAM ────────────────────────────────────────────────
public record CreateOdontogramCommand(CreateOdontogramDto Odontogram) : IRequest<Result<OdontogramDto>>;

public class CreateOdontogramCommandHandler : IRequestHandler<CreateOdontogramCommand, Result<OdontogramDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public CreateOdontogramCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<OdontogramDto>> Handle(CreateOdontogramCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Odontogram;

        var patient = await _unitOfWork.Patients.GetByIdAsync(dto.PatientId, cancellationToken)
            ?? throw new NotFoundException(nameof(Patient), dto.PatientId);

        var odontogram = new Domain.Entities.Odontogram
        {
            PatientId = dto.PatientId,
            DoctorId = dto.DoctorId,
            ExaminationDate = dto.ExaminationDate ?? DateTime.UtcNow,
            Notes = dto.Notes,
            CreatedBy = _currentUser.UserId
        };

        // Initialize all teeth (FDI notation)
        var toothNumbers = dto.IsPediatric
            ? new[] { 51, 52, 53, 54, 55, 61, 62, 63, 64, 65, 71, 72, 73, 74, 75, 81, 82, 83, 84, 85 }
            : new[] { 11, 12, 13, 14, 15, 16, 17, 18, 21, 22, 23, 24, 25, 26, 27, 28,
                      31, 32, 33, 34, 35, 36, 37, 38, 41, 42, 43, 44, 45, 46, 47, 48 };

        foreach (var number in toothNumbers)
        {
            odontogram.TeethRecords.Add(new ToothRecord
            {
                ToothNumber = number,
                ToothType = dto.IsPediatric ? ToothType.Deciduous : ToothType.Permanent,
                Condition = ToothCondition.Healthy,
                IsPresent = true
            });
        }

        await _unitOfWork.Odontograms.AddAsync(odontogram, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<OdontogramDto>.Success(new OdontogramDto
        {
            Id = odontogram.Id,
            PatientId = odontogram.PatientId,
            PatientName = patient.FullName,
            ExaminationDate = odontogram.ExaminationDate,
            Notes = odontogram.Notes,
            DoctorId = odontogram.DoctorId,
            TeethRecords = odontogram.TeethRecords.Select(t => new ToothRecordDto
            {
                Id = t.Id, ToothNumber = t.ToothNumber, ToothType = t.ToothType,
                Condition = t.Condition, IsPresent = t.IsPresent
            }).ToList()
        }, "Odontograma creado.");
    }
}

// ─── UPDATE TOOTH ─────────────────────────────────────────────────────
public record UpdateToothCommand(Guid ToothRecordId, UpdateToothDto Tooth) : IRequest<Result<ToothRecordDto>>;

public class UpdateToothCommandHandler : IRequestHandler<UpdateToothCommand, Result<ToothRecordDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateToothCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ToothRecordDto>> Handle(UpdateToothCommand request, CancellationToken cancellationToken)
    {
        var tooth = await _unitOfWork.ToothRecords.GetByIdAsync(request.ToothRecordId, cancellationToken)
            ?? throw new NotFoundException(nameof(ToothRecord), request.ToothRecordId);

        tooth.Condition = request.Tooth.Condition;
        tooth.IsPresent = request.Tooth.IsPresent;
        tooth.Notes = request.Tooth.Notes;

        await _unitOfWork.ToothRecords.UpdateAsync(tooth, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ToothRecordDto>.Success(new ToothRecordDto
        {
            Id = tooth.Id, ToothNumber = tooth.ToothNumber, ToothType = tooth.ToothType,
            Condition = tooth.Condition, IsPresent = tooth.IsPresent, Notes = tooth.Notes
        }, "Diente actualizado.");
    }
}

// ─── ADD SURFACE RECORD ───────────────────────────────────────────────
public record AddSurfaceRecordCommand(Guid ToothRecordId, AddSurfaceDto Surface) : IRequest<Result<ToothSurfaceDto>>;

public class AddSurfaceRecordCommandHandler : IRequestHandler<AddSurfaceRecordCommand, Result<ToothSurfaceDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddSurfaceRecordCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ToothSurfaceDto>> Handle(AddSurfaceRecordCommand request, CancellationToken cancellationToken)
    {
        var tooth = await _unitOfWork.ToothRecords.GetByIdAsync(request.ToothRecordId, cancellationToken)
            ?? throw new NotFoundException(nameof(ToothRecord), request.ToothRecordId);

        // Check if surface already exists — update it
        var existing = (await _unitOfWork.ToothSurfaceRecords.FindAsync(
            s => s.ToothRecordId == request.ToothRecordId && s.Surface == request.Surface.Surface, cancellationToken))
            .FirstOrDefault();

        if (existing != null)
        {
            existing.Condition = request.Surface.Condition;
            existing.Notes = request.Surface.Notes;
            await _unitOfWork.ToothSurfaceRecords.UpdateAsync(existing, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<ToothSurfaceDto>.Success(new ToothSurfaceDto
            {
                Id = existing.Id, Surface = existing.Surface,
                Condition = existing.Condition, Notes = existing.Notes
            }, "Superficie actualizada.");
        }

        var surfaceRecord = new ToothSurfaceRecord
        {
            ToothRecordId = request.ToothRecordId,
            Surface = request.Surface.Surface,
            Condition = request.Surface.Condition,
            Notes = request.Surface.Notes
        };

        await _unitOfWork.ToothSurfaceRecords.AddAsync(surfaceRecord, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ToothSurfaceDto>.Success(new ToothSurfaceDto
        {
            Id = surfaceRecord.Id, Surface = surfaceRecord.Surface,
            Condition = surfaceRecord.Condition, Notes = surfaceRecord.Notes
        }, "Superficie registrada.");
    }
}

// ─── ADD TREATMENT RECORD ─────────────────────────────────────────────
public record AddTreatmentRecordCommand(Guid ToothRecordId, AddTreatmentRecordDto Treatment) : IRequest<Result<TreatmentRecordDto>>;

public class AddTreatmentRecordCommandHandler : IRequestHandler<AddTreatmentRecordCommand, Result<TreatmentRecordDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public AddTreatmentRecordCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<TreatmentRecordDto>> Handle(AddTreatmentRecordCommand request, CancellationToken cancellationToken)
    {
        var tooth = await _unitOfWork.ToothRecords.GetByIdAsync(request.ToothRecordId, cancellationToken)
            ?? throw new NotFoundException(nameof(ToothRecord), request.ToothRecordId);

        var treatment = await _unitOfWork.Treatments.GetByIdAsync(request.Treatment.TreatmentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Treatment), request.Treatment.TreatmentId);

        var doctor = await _unitOfWork.Doctors.GetByIdAsync(request.Treatment.DoctorId, cancellationToken)
            ?? throw new NotFoundException(nameof(Doctor), request.Treatment.DoctorId);

        var record = new TreatmentRecord
        {
            ToothRecordId = request.ToothRecordId,
            TreatmentId = request.Treatment.TreatmentId,
            DoctorId = request.Treatment.DoctorId,
            AppointmentId = request.Treatment.AppointmentId,
            PerformedDate = request.Treatment.PerformedDate ?? DateTime.UtcNow,
            Price = request.Treatment.Price ?? treatment.DefaultPrice,
            Notes = request.Treatment.Notes,
            SurfacesAffected = request.Treatment.SurfacesAffected,
            IsCompleted = request.Treatment.IsCompleted,
            CreatedBy = _currentUser.UserId
        };

        await _unitOfWork.TreatmentRecords.AddAsync(record, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<TreatmentRecordDto>.Success(new TreatmentRecordDto
        {
            Id = record.Id,
            ToothRecordId = record.ToothRecordId,
            ToothNumber = tooth.ToothNumber,
            TreatmentId = record.TreatmentId,
            TreatmentName = treatment.Name,
            TreatmentCode = treatment.Code,
            DoctorId = record.DoctorId,
            DoctorName = doctor.FullName,
            PerformedDate = record.PerformedDate,
            Price = record.Price,
            Notes = record.Notes,
            SurfacesAffected = record.SurfacesAffected,
            IsCompleted = record.IsCompleted
        }, "Tratamiento registrado.");
    }
}
