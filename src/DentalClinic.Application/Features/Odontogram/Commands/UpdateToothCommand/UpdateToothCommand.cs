using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Odontogram.DTOs;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Odontogram.Commands.UpdateToothCommand;

public record UpdateToothCommand(Guid ToothRecordId, UpdateToothDto Tooth) : IRequest<Result<ToothRecordDto>>;

public class UpdateToothCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateToothCommand, Result<ToothRecordDto>>
{
    public async Task<Result<ToothRecordDto>> Handle(UpdateToothCommand request, CancellationToken cancellationToken)
    {
        var tooth = await unitOfWork.ToothRecords.GetByIdAsync(request.ToothRecordId, cancellationToken)
                    ?? throw new NotFoundException(nameof(ToothRecord), request.ToothRecordId);

        tooth.Condition = request.Tooth.Condition;
        tooth.IsPresent = request.Tooth.IsPresent;
        tooth.Notes = request.Tooth.Notes;

        await unitOfWork.ToothRecords.UpdateAsync(tooth, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ToothRecordDto>.Success(new ToothRecordDto
        {
            Id = tooth.Id, ToothNumber = tooth.ToothNumber, ToothType = tooth.ToothType,
            Condition = tooth.Condition, IsPresent = tooth.IsPresent, Notes = tooth.Notes
        }, "Diente actualizado.");
    }
}