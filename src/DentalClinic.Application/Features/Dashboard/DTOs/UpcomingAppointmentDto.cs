namespace DentalClinic.Application.Features.Dashboard.DTOs;

public class UpcomingAppointmentDto
{
    public Guid Id { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public DateTime ScheduledEndDate { get; set; }
    public int Status { get; set; }
    public string? Reason { get; set; }
}