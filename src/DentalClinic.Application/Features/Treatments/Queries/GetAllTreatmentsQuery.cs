// src/DentalClinic.Application/Features/Treatments/Queries/GetAllTreatmentsQuery.cs
using AutoMapper;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Treatments.DTOs;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Treatments.Queries;

public record GetAllTreatmentsQuery : IRequest<Result<IReadOnlyList<TreatmentDto>>>;

public class GetAllTreatmentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetAllTreatmentsQuery, Result<IReadOnlyList<TreatmentDto>>>
{
    public async Task<Result<IReadOnlyList<TreatmentDto>>> Handle(GetAllTreatmentsQuery request, CancellationToken ct)
    {
        var treatments = await unitOfWork.Treatments.FindWithIncludeAsync(
            t => t.IsActive,
            ct,
            t => t.Category
        );
        
        var dtos = mapper.Map<IReadOnlyList<TreatmentDto>>(treatments);

        return Result<IReadOnlyList<TreatmentDto>>.Success(dtos);
    }
}