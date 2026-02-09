using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.TreatmentCategories.DTOs;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.TreatmentCategories.Commands.CreateTreatmentCategoryCommand;

public record CreateTreatmentCategoryCommand(CreateTreatmentCategoryDto Category) : IRequest<Result<TreatmentCategoryDto>>;

public class CreateTreatmentCategoryHandler(IUnitOfWork uow)
    : IRequestHandler<CreateTreatmentCategoryCommand, Result<TreatmentCategoryDto>>
{
    public async Task<Result<TreatmentCategoryDto>> Handle(CreateTreatmentCategoryCommand request, CancellationToken ct)
    {
        var dto = request.Category;
        var entity = new TreatmentCategory
        {
            Name = dto.Name, Description = dto.Description,
            Color = dto.Color, IsActive = true
        };
        await uow.TreatmentCategories.AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);
        return Result<TreatmentCategoryDto>.Success(new TreatmentCategoryDto
        {
            Id = entity.Id, Name = entity.Name, Description = entity.Description,
            Color = entity.Color, IsActive = entity.IsActive
        }, "Categoría creada.");
    }
}