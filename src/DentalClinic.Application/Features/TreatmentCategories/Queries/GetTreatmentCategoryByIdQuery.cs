using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.TreatmentCategories.DTOs;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.TreatmentCategories.Queries;

public record GetTreatmentCategoryByIdQuery(Guid Id) : IRequest<Result<TreatmentCategoryDto>>;

public class GetTreatmentCategoryByIdHandler(IUnitOfWork uow)
    : IRequestHandler<GetTreatmentCategoryByIdQuery, Result<TreatmentCategoryDto>>
{
    public async Task<Result<TreatmentCategoryDto>> Handle(GetTreatmentCategoryByIdQuery request, CancellationToken ct)
    {
        var c = await uow.TreatmentCategories.GetByIdAsync(request.Id, ct)
                ?? throw new NotFoundException(nameof(TreatmentCategory), request.Id);
        return Result<TreatmentCategoryDto>.Success(new TreatmentCategoryDto
        {
            Id = c.Id, Name = c.Name, Description = c.Description,
            Color = c.Color, IsActive = c.IsActive
        });
    }
}