using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Interfaces;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Odontogram.DTOs;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Enums;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Odontogram.Commands.CreateOdontogramCommand;

public record CreateOdontogramCommand(CreateOdontogramDto Odontogram) : IRequest<Result<OdontogramDto>>;

public class CreateOdontogramCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<CreateOdontogramCommand, Result<OdontogramDto>>
{
    public async Task<Result<OdontogramDto>> Handle(CreateOdontogramCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Odontogram;

        var patient = await unitOfWork.Patients.GetByIdAsync(dto.PatientId, cancellationToken)
            ?? throw new NotFoundException(nameof(Patient), dto.PatientId);

        var odontogram = new Domain.Entities.Odontogram
        {
            PatientId = dto.PatientId,
            DoctorId = dto.DoctorId,
            ExaminationDate = dto.ExaminationDate ?? DateTime.UtcNow,
            Notes = dto.Notes,
            CreatedBy = currentUser.UserId
        };

        // Initialize all teeth (FDI notation)
        var toothNumbers = dto.IsPediatric
            ? new[] { 51, 52, 53, 54, 55, 61, 62, 63, 64, 65, 71, 72, 73, 74, 75, 81, 82, 83, 84, 85 }
            : new[] { 11, 12, 13, 14, 15, 16, 17, 18, 21, 22, 23, 24, 25, 26, 27, 28,
                      31, 32, 33, 34, 35, 36, 37, 38, 41, 42, 43, 44, 45, 46, 47, 48 };

        // Todas las superficies del enum
        var allSurfaces = new[]
        {
            ToothSurface.Mesial,
            ToothSurface.Distal,
            ToothSurface.Buccal,
            ToothSurface.Lingual,
            ToothSurface.Oclusal,
            ToothSurface.Incisal
        };

        foreach (var number in toothNumbers)
        {
            var toothRecord = new ToothRecord
            {
                ToothNumber = number,
                ToothType = dto.IsPediatric ? ToothType.Deciduous : ToothType.Permanent,
                Condition = ToothCondition.Healthy,
                IsPresent = true
            };

            // Crear todas las 6 superficies para cada diente
            foreach (var surface in allSurfaces)
            {
                toothRecord.SurfaceRecords.Add(new ToothSurfaceRecord
                {
                    Surface = surface,
                    Condition = ToothCondition.Healthy
                });
            }

            odontogram.TeethRecords.Add(toothRecord);
        }

        await unitOfWork.Odontograms.AddAsync(odontogram, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

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
                Id = t.Id,
                ToothNumber = t.ToothNumber,
                ToothType = t.ToothType,
                Condition = t.Condition,
                IsPresent = t.IsPresent,
                Surfaces = t.SurfaceRecords.Select(s => new ToothSurfaceDto
                {
                    Id = s.Id,
                    Surface = s.Surface,
                    Condition = s.Condition,
                    Notes = s.Notes
                }).ToList()
            }).ToList()
        }, "Odontograma creado.");
    }
}