using DentalClinic.Domain.Common;
using DentalClinic.Domain.Enums;

namespace DentalClinic.Domain.Entities;

/// <summary>
/// Catalog of available dental treatments/procedures.
/// </summary>
public class Treatment : BaseAuditableEntity
{
    public string Code { get; set; } = string.Empty;      
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public TreatmentCategory Category { get; set; }
    public decimal DefaultPrice { get; set; }
    public int? EstimatedDurationMinutes { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsGlobalTreatment { get; set; } = false;
    

    // Navigation
    public ICollection<TreatmentRecord> TreatmentRecords { get; set; } = new List<TreatmentRecord>();
}
