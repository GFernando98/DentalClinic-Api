using DentalClinic.Domain.Entities;

namespace DentalClinic.Application.Features.Treatments.DTOs;

public class CreateTreatmentDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public decimal DefaultPrice { get; set; }
    public int? EstimatedDurationMinutes { get; set; }
}