using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Odontogram.DTOs;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Odontogram.Commands.AddSurfaceRecordCommand;

public record AddSurfaceRecordCommand(Guid ToothRecordId, AddSurfaceDto Surface) : IRequest<Result<ToothSurfaceDto>>;

public class AddSurfaceRecordCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<AddSurfaceRecordCommand, Result<ToothSurfaceDto>>
{
    public async Task<Result<ToothSurfaceDto>> Handle(AddSurfaceRecordCommand request, CancellationToken cancellationToken)
    {
        var tooth = await unitOfWork.ToothRecords.GetByIdAsync(request.ToothRecordId, cancellationToken)
            ?? throw new NotFoundException(nameof(ToothRecord), request.ToothRecordId);

        // Check if surface already exists — update it
        var existing = (await unitOfWork.ToothSurfaceRecords.FindAsync(
            s => s.ToothRecordId == request.ToothRecordId && s.Surface == request.Surface.Surface, cancellationToken))
            .FirstOrDefault();

        if (existing != null)
        {
            existing.Condition = request.Surface.Condition;
            existing.Notes = request.Surface.Notes;
            await unitOfWork.ToothSurfaceRecords.UpdateAsync(existing, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

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

        await unitOfWork.ToothSurfaceRecords.AddAsync(surfaceRecord, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ToothSurfaceDto>.Success(new ToothSurfaceDto
        {
            Id = surfaceRecord.Id, Surface = surfaceRecord.Surface,
            Condition = surfaceRecord.Condition, Notes = surfaceRecord.Notes
        }, "Superficie registrada.");
    }
}
