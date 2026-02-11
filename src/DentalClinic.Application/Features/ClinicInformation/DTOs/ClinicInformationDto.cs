namespace DentalClinic.Application.Features.ClinicInformation.DTOs;

public class ClinicInformationDto
{
    public Guid Id { get; set; }
    public string ClinicName { get; set; } = string.Empty;
    public string LegalName { get; set; } = string.Empty;
    public string RTN { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Website { get; set; }
    public byte[]? Logo { get; set; }
    public bool IsActive { get; set; }
}