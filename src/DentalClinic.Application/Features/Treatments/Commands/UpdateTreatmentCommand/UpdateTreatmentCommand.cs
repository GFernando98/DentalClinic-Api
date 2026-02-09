using AutoMapper;
using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Treatments.DTOs;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Treatments.Commands.UpdateTreatmentCommand;

public record UpdateTreatmentCommand(Guid Id, CreateTreatmentDto Treatment) : IRequest<Result<TreatmentDto>>;

public class UpdateTreatmentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<UpdateTreatmentCommand, Result<TreatmentDto>>
{
    public async Task<Result<TreatmentDto>> Handle(UpdateTreatmentCommand request, CancellationToken ct)
    {
        var treatment = await unitOfWork.Treatments.GetByIdAsync(request.Id, ct)
                        ?? throw new NotFoundException(nameof(Treatment), request.Id);

        var dto = request.Treatment;
        
        var codeExists = await unitOfWork.Treatments.ExistsAsync(
            t => t.Code == dto.Code && t.Id != request.Id, ct);
        if (codeExists)
            return Result<TreatmentDto>.Failure("Ya existe otro tratamiento con ese código.");
        
        var categoryExists = await unitOfWork.TreatmentCategories.ExistsAsync(
            c => c.Id == dto.CategoryId, ct);
        if (!categoryExists)
            return Result<TreatmentDto>.Failure("La categoría especificada no existe.");
        
        treatment.Code = dto.Code;
        treatment.Name = dto.Name;
        treatment.Description = dto.Description;
        treatment.CategoryId = dto.CategoryId;
        treatment.DefaultPrice = dto.DefaultPrice;
        treatment.EstimatedDurationMinutes = dto.EstimatedDurationMinutes;
        treatment.IsGlobalTreatment = dto.IsGlobalTreatment;

        await unitOfWork.Treatments.UpdateAsync(treatment, ct);
        await unitOfWork.SaveChangesAsync(ct);
        
        var updatedTreatment = await unitOfWork.Treatments.FindWithIncludeAsync(
            t => t.Id == treatment.Id,
            ct,
            t => t.Category
        );

        var result = mapper.Map<TreatmentDto>(updatedTreatment.First());

        return Result<TreatmentDto>.Success(result, "Tratamiento actualizado exitosamente.");
    }
}