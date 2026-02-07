using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Interfaces;
using DentalClinic.Application.Common.Models;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Enums;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Treatments;

// ─── DTOs ─────────────────────────────────────────────────────────────
public class TreatmentDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TreatmentCategory Category { get; set; }
    public decimal DefaultPrice { get; set; }
    public int? EstimatedDurationMinutes { get; set; }
    public bool IsActive { get; set; }
}

public class CreateTreatmentDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TreatmentCategory Category { get; set; }
    public decimal DefaultPrice { get; set; }
    public int? EstimatedDurationMinutes { get; set; }
}

// ─── QUERIES ──────────────────────────────────────────────────────────
public record GetAllTreatmentsQuery : IRequest<Result<IReadOnlyList<TreatmentDto>>>;

public class GetAllTreatmentsQueryHandler : IRequestHandler<GetAllTreatmentsQuery, Result<IReadOnlyList<TreatmentDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetAllTreatmentsQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<IReadOnlyList<TreatmentDto>>> Handle(GetAllTreatmentsQuery request, CancellationToken ct)
    {
        var treatments = await _unitOfWork.Treatments.FindAsync(t => t.IsActive, ct);
        var dtos = treatments.Select(t => new TreatmentDto
        {
            Id = t.Id, Code = t.Code, Name = t.Name, Description = t.Description,
            Category = t.Category, DefaultPrice = t.DefaultPrice,
            EstimatedDurationMinutes = t.EstimatedDurationMinutes, IsActive = t.IsActive
        }).ToList().AsReadOnly();

        return Result<IReadOnlyList<TreatmentDto>>.Success(dtos);
    }
}

public record GetTreatmentByIdQuery(Guid Id) : IRequest<Result<TreatmentDto>>;

public class GetTreatmentByIdQueryHandler : IRequestHandler<GetTreatmentByIdQuery, Result<TreatmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetTreatmentByIdQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<TreatmentDto>> Handle(GetTreatmentByIdQuery request, CancellationToken ct)
    {
        var t = await _unitOfWork.Treatments.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Treatment), request.Id);

        return Result<TreatmentDto>.Success(new TreatmentDto
        {
            Id = t.Id, Code = t.Code, Name = t.Name, Description = t.Description,
            Category = t.Category, DefaultPrice = t.DefaultPrice,
            EstimatedDurationMinutes = t.EstimatedDurationMinutes, IsActive = t.IsActive
        });
    }
}

// ─── COMMANDS ─────────────────────────────────────────────────────────
public record CreateTreatmentCommand(CreateTreatmentDto Treatment) : IRequest<Result<TreatmentDto>>;

public class CreateTreatmentCommandHandler : IRequestHandler<CreateTreatmentCommand, Result<TreatmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateTreatmentCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<TreatmentDto>> Handle(CreateTreatmentCommand request, CancellationToken ct)
    {
        var dto = request.Treatment;

        var codeExists = await _unitOfWork.Treatments.ExistsAsync(t => t.Code == dto.Code, ct);
        if (codeExists)
            return Result<TreatmentDto>.Failure("Ya existe un tratamiento con ese código.");

        var treatment = new Treatment
        {
            Code = dto.Code, Name = dto.Name, Description = dto.Description,
            Category = dto.Category, DefaultPrice = dto.DefaultPrice,
            EstimatedDurationMinutes = dto.EstimatedDurationMinutes, IsActive = true
        };

        await _unitOfWork.Treatments.AddAsync(treatment, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<TreatmentDto>.Success(new TreatmentDto
        {
            Id = treatment.Id, Code = treatment.Code, Name = treatment.Name,
            Description = treatment.Description, Category = treatment.Category,
            DefaultPrice = treatment.DefaultPrice,
            EstimatedDurationMinutes = treatment.EstimatedDurationMinutes,
            IsActive = treatment.IsActive
        }, "Tratamiento creado.");
    }
}

public record UpdateTreatmentCommand(Guid Id, CreateTreatmentDto Treatment) : IRequest<Result<TreatmentDto>>;

public class UpdateTreatmentCommandHandler : IRequestHandler<UpdateTreatmentCommand, Result<TreatmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTreatmentCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<TreatmentDto>> Handle(UpdateTreatmentCommand request, CancellationToken ct)
    {
        var treatment = await _unitOfWork.Treatments.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Treatment), request.Id);

        var dto = request.Treatment;
        treatment.Code = dto.Code;
        treatment.Name = dto.Name;
        treatment.Description = dto.Description;
        treatment.Category = dto.Category;
        treatment.DefaultPrice = dto.DefaultPrice;
        treatment.EstimatedDurationMinutes = dto.EstimatedDurationMinutes;

        await _unitOfWork.Treatments.UpdateAsync(treatment, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<TreatmentDto>.Success(new TreatmentDto
        {
            Id = treatment.Id, Code = treatment.Code, Name = treatment.Name,
            Description = treatment.Description, Category = treatment.Category,
            DefaultPrice = treatment.DefaultPrice,
            EstimatedDurationMinutes = treatment.EstimatedDurationMinutes,
            IsActive = treatment.IsActive
        }, "Tratamiento actualizado.");
    }
}
