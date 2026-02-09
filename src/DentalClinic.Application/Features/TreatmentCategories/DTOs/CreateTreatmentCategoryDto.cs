namespace DentalClinic.Application.Features.TreatmentCategories.DTOs;

public class CreateTreatmentCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Color { get; set; }
}