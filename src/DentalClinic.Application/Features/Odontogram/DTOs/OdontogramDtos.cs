using DentalClinic.Domain.Enums;

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

public class ToothRecordDto
{
    public Guid Id { get; set; }
    public int ToothNumber { get; set; }
    public ToothType ToothType { get; set; }
    public ToothCondition Condition { get; set; }
    public bool IsPresent { get; set; }
    public string? Notes { get; set; }
    public List<ToothSurfaceDto> Surfaces { get; set; } = new();
}

public class ToothSurfaceDto
{
    public Guid Id { get; set; }
    public ToothSurface Surface { get; set; }
    public ToothCondition Condition { get; set; }
    public string? Notes { get; set; }
}

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
}

public class CreateOdontogramDto
{
    public Guid PatientId { get; set; }
    public Guid? DoctorId { get; set; }
    public DateTime? ExaminationDate { get; set; }
    public string? Notes { get; set; }
    public bool IsPediatric { get; set; }
}

public class UpdateToothDto
{
    public ToothCondition Condition { get; set; }
    public bool IsPresent { get; set; } = true;
    public string? Notes { get; set; }
}

public class AddSurfaceDto
{
    public ToothSurface Surface { get; set; }
    public ToothCondition Condition { get; set; }
    public string? Notes { get; set; }
}

public class AddTreatmentRecordDto
{
    public Guid TreatmentId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid? AppointmentId { get; set; }
    public DateTime? PerformedDate { get; set; }
    public decimal? Price { get; set; }
    public string? Notes { get; set; }
    public string? SurfacesAffected { get; set; }
    public bool IsCompleted { get; set; } = true;
}
