namespace DentalClinic.Domain.Enums;

public enum Gender
{
    Male = 1,
    Female = 2,
    Other = 3
}

public enum ToothQuadrant
{
    UpperRight = 1,  // Cuadrante 1
    UpperLeft = 2,   // Cuadrante 2
    LowerLeft = 3,   // Cuadrante 3
    LowerRight = 4   // Cuadrante 4
}

public enum ToothType
{
    Permanent = 1,
    Deciduous = 2  // Dientes de leche
}

public enum ToothCondition
{
    Healthy = 1,
    Decayed = 2,           // Cariado
    Filled = 3,            // Obturado
    Missing = 4,           // Ausente
    Extracted = 5,         // Extraído
    Crown = 6,             // Corona
    Bridge = 7,            // Puente
    Implant = 8,           // Implante
    RootCanal = 9,         // Endodoncia
    Fracture = 10,         // Fractura
    Sealant = 11,          // Sellante
    Prosthesis = 12        // Prótesis
}

public enum ToothSurface
{
    Mesial = 1,
    Distal = 2,
    Buccal = 3,      // Vestibular
    Lingual = 4,
    Oclusal = 5,    // Oclusal (solo en molares/premolares)
    Incisal = 6      // Incisal (solo en incisivos/caninos)
}

public enum AppointmentStatus
{
    Scheduled = 1,     // Programada
    Confirmed = 2,     // Confirmada
    InProgress = 3,    // En progreso
    Completed = 4,     // Completada
    Cancelled = 5,     // Cancelada
    NoShow = 6         // No se presentó
}

public enum NotificationType
{
    AppointmentReminder = 1,
    AppointmentConfirmation = 2,
    AppointmentCancellation = 3,
    TreatmentFollowUp = 4,
    Birthday = 5,
    General = 6
}

public enum NotificationChannel
{
    Email = 1,
    WhatsApp = 2,
    Both = 3
}

public enum NotificationStatus
{
    Pending = 1,
    Sent = 2,
    Failed = 3,
    Delivered = 4
}
