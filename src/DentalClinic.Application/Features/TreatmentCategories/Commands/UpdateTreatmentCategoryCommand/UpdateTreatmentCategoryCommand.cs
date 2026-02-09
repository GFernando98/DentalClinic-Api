using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.TreatmentCategories.DTOs;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.TreatmentCategories.Commands.UpdateTreatmentCategoryCommand;

public record UpdateTreatmentCategoryCommand(Guid Id, CreateTreatmentCategoryDto Category) : IRequest<Result<TreatmentCategoryDto>>;

public class UpdateTreatmentCategoryHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateTreatmentCategoryCommand, Result<TreatmentCategoryDto>>
{
    public async Task<Result<TreatmentCategoryDto>> Handle(UpdateTreatmentCategoryCommand request, CancellationToken ct)
    {
        var entity = await uow.TreatmentCategories.GetByIdAsync(request.Id, ct)
                     ?? throw new NotFoundException(nameof(TreatmentCategory), request.Id);
        entity.Name = request.Category.Name;
        entity.Description = request.Category.Description;
        entity.Color = request.Category.Color;
        await uow.TreatmentCategories.UpdateAsync(entity, ct);
        await uow.SaveChangesAsync(ct);
        return Result<TreatmentCategoryDto>.Success(new TreatmentCategoryDto
        {
            Id = entity.Id, Name = entity.Name, Description = entity.Description,
            Color = entity.Color, IsActive = entity.IsActive
        }, "Categoría actualizada.");
    }
}