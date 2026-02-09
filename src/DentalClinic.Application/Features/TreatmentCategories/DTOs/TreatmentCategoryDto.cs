namespace DentalClinic.Application.Features.TreatmentCategories.DTOs;

public class TreatmentCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Color { get; set; }
    public bool IsActive { get; set; }
}