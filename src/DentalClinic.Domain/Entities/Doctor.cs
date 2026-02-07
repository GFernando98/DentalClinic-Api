using DentalClinic.Domain.Common;

namespace DentalClinic.Domain.Entities;

public class Doctor : BaseAuditableEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? LicenseNumber { get; set; }   // Número de colegiado
    public string? Specialty { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public bool IsActive { get; set; } = true;

    // Link to Identity
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }

    // Navigation
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<TreatmentRecord> TreatmentRecords { get; set; } = new List<TreatmentRecord>();

    public string FullName => $"{FirstName} {LastName}";
}
