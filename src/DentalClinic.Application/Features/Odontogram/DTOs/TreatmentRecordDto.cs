namespace DentalClinic.Application.Features.Odontogram.DTOs;

public class TreatmentRecordDto
{
    public Guid Id { get; set; }
    public Guid ToothRecordId { get; set; }
    public int ToothNumber { get; set; }
    public Guid TreatmentId { get; set; }
    public string TreatmentName { get; set; } = string.Empty;
    public string? TreatmentCode { get; set; }
    public Guid DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public DateTime PerformedDate { get; set; }
    public decimal Price { get; set; }
    public string? Notes { get; set; }
    public string? SurfacesAffected { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsPaid { get; set; }
    public bool IsGlobalTreatment { get; set; }
}