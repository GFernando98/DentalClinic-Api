using DentalClinic.Domain.Enums;

namespace DentalClinic.Application.Features.Appointments.DTOs;

public class AppointmentDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public Guid DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public DateTime ScheduledEndDate { get; set; }
    public AppointmentStatus Status { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public string? CancellationReason { get; set; }
    public bool ReminderSent { get; set; }
}

public class CreateAppointmentDto
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public DateTime ScheduledEndDate { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}

public class UpdateAppointmentStatusDto
{
    public AppointmentStatus Status { get; set; }
    public string? CancellationReason { get; set; }
}
