namespace DentalClinic.Domain.Enums;
public enum InvoiceStatus
{
    Pending = 1,      // Pendiente de pago
    PartiallyPaid = 2,  // Parcialmente pagada
    Paid = 3,         // Pagada completamente
    Cancelled = 4,    // Cancelada
    Overdue = 5       // Vencida
}