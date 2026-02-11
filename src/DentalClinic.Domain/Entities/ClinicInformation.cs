using DentalClinic.Domain.Common;

namespace DentalClinic.Domain.Entities;

public class ClinicInformation : BaseAuditableEntity
{
    public string ClinicName { get; set; } = string.Empty;
    public string LegalName { get; set; } = string.Empty;
    public string RTN { get; set; } = string.Empty;  // Registro Tributario Nacional
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Country { get; set; } = "Honduras";
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Website { get; set; }
    public byte[]? Logo { get; set; } 
    public bool IsActive { get; set; } = true;
}