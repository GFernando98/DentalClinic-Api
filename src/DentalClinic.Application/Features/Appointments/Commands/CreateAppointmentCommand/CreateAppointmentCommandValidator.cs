using FluentValidation;

namespace DentalClinic.Application.Features.Appointments.Commands.CreateAppointmentCommand;

public class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
{
    public CreateAppointmentCommandValidator()
    {
        RuleFor(x => x.Appointment.PatientId)
            .NotEmpty().WithMessage("El paciente es requerido.");

        RuleFor(x => x.Appointment.DoctorId)
            .NotEmpty().WithMessage("El doctor es requerido.");

        RuleFor(x => x.Appointment.ScheduledDate)
            .GreaterThan(DateTime.UtcNow.AddMinutes(-5))
            .WithMessage("La fecha de la cita debe ser futura.");

        RuleFor(x => x.Appointment.ScheduledEndDate)
            .GreaterThan(x => x.Appointment.ScheduledDate)
            .WithMessage("La fecha de fin debe ser posterior a la de inicio.");
    }
}
