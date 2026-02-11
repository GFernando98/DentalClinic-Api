using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Models;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.TaxInformation.Commands.DeactivateTaxInformationCommand;

public record DeactivateTaxInformationCommand(Guid Id) : IRequest<Result<bool>>;

public class DeactivateTaxInformationCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeactivateTaxInformationCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeactivateTaxInformationCommand request, CancellationToken ct)
    {
        var taxInfo = await unitOfWork.TaxInformation.GetByIdAsync(request.Id, ct)
                      ?? throw new NotFoundException(nameof(Domain.Entities.TaxInformation), request.Id);

        taxInfo.IsActive = false;

        await unitOfWork.TaxInformation.UpdateAsync(taxInfo, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result<bool>.Success(true, "CAI desactivado.");
    }
}