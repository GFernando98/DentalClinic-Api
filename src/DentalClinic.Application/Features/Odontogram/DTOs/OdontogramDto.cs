namespace DentalClinic.Application.Features.Odontogram.DTOs;

public class OdontogramDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public DateTime ExaminationDate { get; set; }
    public string? Notes { get; set; }
    public Guid? DoctorId { get; set; }
    public string? DoctorName { get; set; }
    public List<ToothRecordDto> TeethRecords { get; set; } = new();
}