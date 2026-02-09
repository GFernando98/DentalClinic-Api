using AutoMapper;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Treatments.DTOs;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Treatments.Commands.CreateTreatmentCommand;

public record CreateTreatmentCommand(CreateTreatmentDto Treatment) : IRequest<Result<TreatmentDto>>;

public class CreateTreatmentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<CreateTreatmentCommand, Result<TreatmentDto>>
{
    public async Task<Result<TreatmentDto>> Handle(CreateTreatmentCommand request, CancellationToken ct)
    {
        var dto = request.Treatment;
        
        var codeExists = await unitOfWork.Treatments.ExistsAsync(t => t.Code == dto.Code, ct);
        if (codeExists)
            return Result<TreatmentDto>.Failure("Ya existe un tratamiento con ese código.");
        
        var categoryExists = await unitOfWork.TreatmentCategories.ExistsAsync(
            c => c.Id == dto.CategoryId, ct);
        if (!categoryExists)
            return Result<TreatmentDto>.Failure("La categoría especificada no existe.");
        
        var treatment = new Treatment
        {
            Code = dto.Code,
            Name = dto.Name,
            Description = dto.Description,
            CategoryId = dto.CategoryId,
            DefaultPrice = dto.DefaultPrice,
            EstimatedDurationMinutes = dto.EstimatedDurationMinutes,
            IsActive = true
        };

        await unitOfWork.Treatments.AddAsync(treatment, ct);
        await unitOfWork.SaveChangesAsync(ct);
        
        var createdTreatment = await unitOfWork.Treatments.FindWithIncludeAsync(
            t => t.Id == treatment.Id,
            ct,
            t => t.Category
        );

        var result = mapper.Map<TreatmentDto>(createdTreatment.First());

        return Result<TreatmentDto>.Success(result, "Tratamiento creado exitosamente.");
    }
}