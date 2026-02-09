using DentalClinic.Domain.Enums;

namespace DentalClinic.Application.Features.Odontogram.DTOs;

public class InitialToothDataDto
{
    public int ToothNumber { get; set; }
    public ToothCondition Condition { get; set; }
    public bool IsPresent { get; set; }
    public string? Notes { get; set; }
    public List<InitialSurfaceDto>? Surfaces { get; set; }
}