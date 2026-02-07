using DentalClinic.Domain.Common;
using DentalClinic.Domain.Enums;

namespace DentalClinic.Domain.Entities;

/// <summary>
/// Tracks the condition of each surface of a tooth.
/// A tooth can have multiple surfaces affected (mesial, distal, buccal, lingual, occlusal/incisal).
/// </summary>
public class ToothSurfaceRecord : BaseAuditableEntity
{
    public Guid ToothRecordId { get; set; }
    public ToothRecord ToothRecord { get; set; } = null!;
    public ToothSurface Surface { get; set; }
    public ToothCondition Condition { get; set; }
    public string? Notes { get; set; }
}
