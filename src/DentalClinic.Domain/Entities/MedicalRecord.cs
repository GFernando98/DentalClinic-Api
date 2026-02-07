using DentalClinic.Domain.Common;

namespace DentalClinic.Domain.Entities;

/// <summary>
/// General medical history entries for a patient (not tooth-specific).
/// </summary>
public class MedicalRecord : BaseAuditableEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;
    public DateTime RecordDate { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Diagnosis { get; set; }
    public string? AttachmentUrl { get; set; }   // For X-rays, photos, etc.
    public Guid? DoctorId { get; set; }
    public Doctor? Doctor { get; set; }
}
