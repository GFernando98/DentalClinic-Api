using DentalClinic.Domain.Common;
using DentalClinic.Domain.Enums;

namespace DentalClinic.Domain.Entities;

public class Appointment : BaseAuditableEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public Guid DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;

    public DateTime ScheduledDate { get; set; }
    public DateTime ScheduledEndDate { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
    public string? Reason { get; set; }           // Motivo de la cita
    public string? Notes { get; set; }
    public string? CancellationReason { get; set; }
    public bool ReminderSent { get; set; }

    // Navigation
    public ICollection<TreatmentRecord> TreatmentRecords { get; set; } = new List<TreatmentRecord>();
}
