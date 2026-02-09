using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Models;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.TreatmentCategories.Commands.ToggleTreatmentCategoryCommand;

public record ToggleTreatmentCategoryCommand(Guid Id) : IRequest<Result<bool>>;

public class ToggleTreatmentCategoryHandler(IUnitOfWork uow)
    : IRequestHandler<ToggleTreatmentCategoryCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(ToggleTreatmentCategoryCommand request, CancellationToken ct)
    {
        var entity = await uow.TreatmentCategories.GetByIdAsync(request.Id, ct)
                     ?? throw new NotFoundException(nameof(TreatmentCategory), request.Id);
        entity.IsActive = !entity.IsActive;
        await uow.TreatmentCategories.UpdateAsync(entity, ct);
        await uow.SaveChangesAsync(ct);
        return Result<bool>.Success(entity.IsActive, entity.IsActive ? "Categoría activada." : "Categoría desactivada.");
    }
}