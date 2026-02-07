using DentalClinic.Domain.Common;

namespace DentalClinic.Domain.Entities;

/// <summary>
/// Represents a dental chart (odontograma) for a patient.
/// Each patient can have multiple odontograms over time to track changes.
/// </summary>
public class Odontogram : BaseAuditableEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;
    public DateTime ExaminationDate { get; set; }
    public string? Notes { get; set; }
    public Guid? DoctorId { get; set; }
    public Doctor? Doctor { get; set; }

    // Navigation
    public ICollection<ToothRecord> TeethRecords { get; set; } = new List<ToothRecord>();
}
