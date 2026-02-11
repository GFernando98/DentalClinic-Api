using DentalClinic.Domain.Common;

namespace DentalClinic.Domain.Entities;

public class InvoiceLineItem : BaseAuditableEntity
{
    public Guid InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = null!;
    
    public Guid TreatmentId { get; set; }
    public Treatment Treatment { get; set; } = null!;
    
    public string Description { get; set; } = string.Empty;  // Nombre del tratamiento
    public string? ToothNumbers { get; set; }  // "16, 26, 36" o null si es global
    public bool IsGlobalTreatment { get; set; }
    
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
    
    // Referencias a los TreatmentRecords que se están facturando
    public string TreatmentRecordIds { get; set; } = string.Empty;  // JSON array de GUIDs
}