namespace DentalClinic.Application.Features.Dashboard.DTOs;

public class TreatmentsByCategoryDto
{
    public string Category { get; set; } = string.Empty;
    public string Color { get; set; } = "#6B7280";
    public int Count { get; set; }
}