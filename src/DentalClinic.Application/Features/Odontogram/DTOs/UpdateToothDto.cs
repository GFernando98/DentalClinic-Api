using DentalClinic.Domain.Enums;

namespace DentalClinic.Application.Features.Odontogram.DTOs;

public class UpdateToothDto
{
    public ToothCondition Condition { get; set; }
    public bool IsPresent { get; set; } = true;
    public string? Notes { get; set; }
}