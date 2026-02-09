using DentalClinic.Domain.Common;

namespace DentalClinic.Domain.Entities;

/// <summary>
/// Records a specific treatment/procedure performed on a tooth.
/// Links to the tooth record and the treatment catalog.
/// </summary>
public class TreatmentRecord : BaseAuditableEntity
{
    public Guid? ToothRecordId { get; set; }
    public Guid OdontogramId { get; set; }
    public Odontogram Odontogram { get; set; } = null!;
    public ToothRecord ToothRecord { get; set; } = null!;

    public Guid TreatmentId { get; set; }
    public Treatment Treatment { get; set; } = null!;

    public Guid DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;

    public Guid? AppointmentId { get; set; }
    public Appointment? Appointment { get; set; }

    public DateTime PerformedDate { get; set; }
    public decimal Price { get; set; }
    public string? Notes { get; set; }
    public string? SurfacesAffected { get; set; }  // Comma-separated: "M,D,O"
    public bool IsCompleted { get; set; } = true;
    public bool IsPaid { get; set; } = false;
}
