namespace DentalClinic.Application.Features.Odontogram.DTOs;

public class AddGlobalTreatmentDto
{
    public Guid TreatmentId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid? AppointmentId { get; set; }
    public DateTime? PerformedDate { get; set; }
    public decimal? Price { get; set; }
    public string? Notes { get; set; }
    public bool IsCompleted { get; set; } = true;
}