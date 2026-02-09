namespace DentalClinic.Application.Features.Doctors.DTOs;

public class DoctorDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? LicenseNumber { get; set; }
    public string? Specialty { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }
}
