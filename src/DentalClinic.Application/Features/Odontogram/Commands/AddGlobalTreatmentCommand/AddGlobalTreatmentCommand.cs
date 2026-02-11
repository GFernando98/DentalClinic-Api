using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Interfaces;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Odontogram.DTOs;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Odontogram.Commands.AddGlobalTreatmentCommand;

public record AddGlobalTreatmentCommand(Guid OdontogramId, AddGlobalTreatmentDto Treatment) 
    : IRequest<Result<TreatmentRecordDto>>;

public class AddGlobalTreatmentCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<AddGlobalTreatmentCommand, Result<TreatmentRecordDto>>
{
    public async Task<Result<TreatmentRecordDto>> Handle(AddGlobalTreatmentCommand request, CancellationToken cancellationToken)
    {
        // Verificar que el odontograma existe
        var odontogram = await unitOfWork.Odontograms.GetByIdAsync(request.OdontogramId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Odontogram), request.OdontogramId);

        var treatment = await unitOfWork.Treatments.GetByIdAsync(request.Treatment.TreatmentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Treatment), request.Treatment.TreatmentId);

        var doctor = await unitOfWork.Doctors.GetByIdAsync(request.Treatment.DoctorId, cancellationToken)
            ?? throw new NotFoundException(nameof(Doctor), request.Treatment.DoctorId);
        
        if (!treatment.IsGlobalTreatment)
        {
            return Result<TreatmentRecordDto>.Failure(
                $"El tratamiento '{treatment.Name}' no es un tratamiento global. Use el endpoint de tratamientos por diente.");
        }

        var record = new TreatmentRecord
        {
            ToothRecordId = null, 
            OdontogramId = request.OdontogramId,
            TreatmentId = request.Treatment.TreatmentId,
            DoctorId = request.Treatment.DoctorId,
            AppointmentId = request.Treatment.AppointmentId,
            PerformedDate = request.Treatment.PerformedDate ?? DateTime.UtcNow,
            Price = request.Treatment.Price ?? treatment.DefaultPrice,
            Notes = request.Treatment.Notes,
            SurfacesAffected = null, 
            IsCompleted = request.Treatment.IsCompleted,
            CreatedBy = currentUser.UserId
        };

        await unitOfWork.TreatmentRecords.AddAsync(record, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<TreatmentRecordDto>.Success(new TreatmentRecordDto
        {
            Id = record.Id,
            ToothRecordId = null,
            ToothNumber = null,
            TreatmentId = record.TreatmentId,
            TreatmentName = treatment.Name,
            TreatmentCode = treatment.Code,
            DoctorId = record.DoctorId,
            DoctorName = doctor.FullName,
            PerformedDate = record.PerformedDate,
            Price = record.Price,
            Notes = record.Notes,
            SurfacesAffected = null,
            IsCompleted = record.IsCompleted,
            IsGlobalTreatment = true
        }, "Tratamiento global registrado.");
    }
}