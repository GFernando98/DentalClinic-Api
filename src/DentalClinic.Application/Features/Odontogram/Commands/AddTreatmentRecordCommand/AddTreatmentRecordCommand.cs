using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Interfaces;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Odontogram.DTOs;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Odontogram.Commands.AddTreatmentRecordCommand;

public record AddTreatmentRecordCommand(Guid ToothRecordId, AddTreatmentRecordDto Treatment) : IRequest<Result<TreatmentRecordDto>>;

public class AddTreatmentRecordCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<AddTreatmentRecordCommand, Result<TreatmentRecordDto>>
{
    public async Task<Result<TreatmentRecordDto>> Handle(AddTreatmentRecordCommand request, CancellationToken cancellationToken)
    {
        var tooth = await unitOfWork.ToothRecords.GetByIdAsync(request.ToothRecordId, cancellationToken)
            ?? throw new NotFoundException(nameof(ToothRecord), request.ToothRecordId);

        var treatment = await unitOfWork.Treatments.GetByIdAsync(request.Treatment.TreatmentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Treatment), request.Treatment.TreatmentId);

        var doctor = await unitOfWork.Doctors.GetByIdAsync(request.Treatment.DoctorId, cancellationToken)
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
            CreatedBy = currentUser.UserId
        };

        await unitOfWork.TreatmentRecords.AddAsync(record, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

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
