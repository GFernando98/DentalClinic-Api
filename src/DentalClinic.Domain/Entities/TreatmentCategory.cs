using DentalClinic.Domain.Common;

namespace DentalClinic.Domain.Entities;

public class TreatmentCategory : BaseAuditableEntity
{
    
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Color { get; set; }
    public bool IsActive { get; set; } = true;
    
    public ICollection<Treatment> Treatments { get; set; } = new List<Treatment>();
}