using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.TreatmentCategories.DTOs;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.TreatmentCategories.Queries;

public record GetAllTreatmentCategoriesQuery : IRequest<Result<IReadOnlyList<TreatmentCategoryDto>>>;

public class GetAllTreatmentCategoriesHandler(IUnitOfWork uow)
    : IRequestHandler<GetAllTreatmentCategoriesQuery, Result<IReadOnlyList<TreatmentCategoryDto>>>
{
    public async Task<Result<IReadOnlyList<TreatmentCategoryDto>>> Handle(GetAllTreatmentCategoriesQuery request, CancellationToken ct)
    {
        var categories = await uow.TreatmentCategories.GetAllAsync(ct);
        var dtos = categories.Select(c => new TreatmentCategoryDto
        {
            Id = c.Id, Name = c.Name, Description = c.Description,
            Color = c.Color, IsActive = c.IsActive
        }).ToList().AsReadOnly();
        return Result<IReadOnlyList<TreatmentCategoryDto>>.Success(dtos);
    }
}