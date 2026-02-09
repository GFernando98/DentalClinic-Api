namespace DentalClinic.Application.Features.Dashboard.DTOs;

public class AppointmentsByDayDto
{
    public string Day { get; set; } = string.Empty;  
    public string DayLabel { get; set; } = string.Empty;
    public int Count { get; set; }
}