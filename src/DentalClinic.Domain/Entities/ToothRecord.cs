using DentalClinic.Domain.Common;
using DentalClinic.Domain.Enums;

namespace DentalClinic.Domain.Entities;

/// <summary>
/// Represents the record for an individual tooth in an odontogram.
/// Uses FDI World Dental Federation notation (ISO 3950).
/// Tooth numbers: 11-18, 21-28, 31-38, 41-48 (permanent)
///                51-55, 61-65, 71-75, 81-85 (deciduous)
/// </summary>
public class ToothRecord : BaseAuditableEntity
{
    public Guid OdontogramId { get; set; }
    public Odontogram Odontogram { get; set; } = null!;

    /// <summary>
    /// FDI tooth number (e.g., 11 = upper right central incisor)
    /// </summary>
    public int ToothNumber { get; set; }

    public ToothType ToothType { get; set; } = ToothType.Permanent;
    public ToothCondition Condition { get; set; } = ToothCondition.Healthy;
    public string? Notes { get; set; }
    public bool IsPresent { get; set; } = true;
    
    public ICollection<TreatmentRecord> Treatments { get; set; } = new List<TreatmentRecord>();
    public ICollection<ToothSurfaceRecord> SurfaceRecords { get; set; } = new List<ToothSurfaceRecord>();
}
