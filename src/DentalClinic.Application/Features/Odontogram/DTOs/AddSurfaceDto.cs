using DentalClinic.Domain.Enums;

namespace DentalClinic.Application.Features.Odontogram.DTOs;

public class AddSurfaceDto
{
    public ToothSurface Surface { get; set; }
    public ToothCondition Condition { get; set; }
    public string? Notes { get; set; }
}