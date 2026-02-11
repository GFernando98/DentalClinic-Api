namespace DentalClinic.Domain.Enums;

public enum PaymentMethod
{
    Cash = 1,         // Efectivo
    CreditCard = 2,   // Tarjeta de crédito
    DebitCard = 3,    // Tarjeta de débito
    BankTransfer = 4, // Transferencia bancaria
    Check = 5,        // Cheque
    Other = 6         // Otro
}