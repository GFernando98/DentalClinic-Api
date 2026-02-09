namespace DentalClinic.Application.Features.Doctors.DTOs;

public class CreateDoctorDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? LicenseNumber { get; set; }
    public string? Specialty { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? UserId { get; set; }
}