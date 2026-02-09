namespace DentalClinic.Application.Features.Dashboard.DTOs;

public class DashboardStatsDto
{
    public int TodayAppointments { get; set; }
    public int TotalPatients { get; set; }
    public int PendingAppointments { get; set; }
    public int MonthTreatments { get; set; }
}