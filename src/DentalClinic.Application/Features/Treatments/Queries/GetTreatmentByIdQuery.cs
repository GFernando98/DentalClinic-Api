using AutoMapper;
using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Treatments.DTOs;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Treatments.Queries;

public record GetTreatmentByIdQuery(Guid Id) : IRequest<Result<TreatmentDto>>;

public class GetTreatmentByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetTreatmentByIdQuery, Result<TreatmentDto>>
{
    public async Task<Result<TreatmentDto>> Handle(GetTreatmentByIdQuery request, CancellationToken ct)
    {
        var treatments = await unitOfWork.Treatments.FindWithIncludeAsync(
            t => t.Id == request.Id,
            ct,
            t => t.Category
        );

        var treatment = treatments.FirstOrDefault()
                        ?? throw new NotFoundException(nameof(Treatment), request.Id);

        var dto = mapper.Map<TreatmentDto>(treatment);

        return Result<TreatmentDto>.Success(dto);
    }
}