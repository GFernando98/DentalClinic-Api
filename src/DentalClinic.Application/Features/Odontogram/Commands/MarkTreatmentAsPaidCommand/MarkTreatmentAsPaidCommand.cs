using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Models;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Odontogram.Commands.MarkTreatmentAsPaidCommand;

public record MarkTreatmentAsPaidCommand(Guid TreatmentRecordId) : IRequest<Result<bool>>;

public class MarkTreatmentAsPaidCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<MarkTreatmentAsPaidCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(MarkTreatmentAsPaidCommand request, CancellationToken ct)
    {
        var treatmentRecord = await unitOfWork.TreatmentRecords.GetByIdAsync(request.TreatmentRecordId, ct)
                              ?? throw new NotFoundException("TreatmentRecord", request.TreatmentRecordId);

        treatmentRecord.IsPaid = true;
        
        await unitOfWork.TreatmentRecords.UpdateAsync(treatmentRecord, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result<bool>.Success(true, "Tratamiento marcado como pagado.");
    }
}