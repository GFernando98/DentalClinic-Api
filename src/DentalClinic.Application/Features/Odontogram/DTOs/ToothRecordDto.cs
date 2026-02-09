using DentalClinic.Domain.Enums;

namespace DentalClinic.Application.Features.Odontogram.DTOs;

public class ToothRecordDto
{
    public Guid Id { get; set; }
    public int ToothNumber { get; set; }
    public ToothType ToothType { get; set; }
    public ToothCondition Condition { get; set; }
    public bool IsPresent { get; set; }
    public string? Notes { get; set; }
    public List<ToothSurfaceDto> Surfaces { get; set; } = new();
}