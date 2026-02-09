using DentalClinic.Application.Features.TreatmentCategories.DTOs;
using DentalClinic.Domain.Entities;

namespace DentalClinic.Application.Features.Treatments.DTOs;

public class TreatmentDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public TreatmentCategoryDto? Category { get; set; }
    public decimal DefaultPrice { get; set; }
    public int? EstimatedDurationMinutes { get; set; }
    public bool IsActive { get; set; }
    public bool IsGlobalTreatment { get; set; }
}