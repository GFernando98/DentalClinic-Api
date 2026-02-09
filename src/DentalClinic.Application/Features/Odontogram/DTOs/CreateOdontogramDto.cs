namespace DentalClinic.Application.Features.Odontogram.DTOs;

public class CreateOdontogramDto
{
    public Guid PatientId { get; set; }
    public Guid? DoctorId { get; set; }
    public DateTime? ExaminationDate { get; set; }
    public string? Notes { get; set; }
    public bool IsPediatric { get; set; }
}