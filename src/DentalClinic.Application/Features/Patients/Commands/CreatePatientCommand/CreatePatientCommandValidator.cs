using FluentValidation;

namespace DentalClinic.Application.Features.Patients.Commands.CreatePatientCommand;

public class CreatePatientCommandValidator : AbstractValidator<CreatePatientCommand>
{
    public CreatePatientCommandValidator()
    {
        RuleFor(x => x.Patient.FirstName)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(100).WithMessage("El nombre no debe exceder 100 caracteres.");

        RuleFor(x => x.Patient.LastName)
            .NotEmpty().WithMessage("El apellido es requerido.")
            .MaximumLength(100).WithMessage("El apellido no debe exceder 100 caracteres.");

        RuleFor(x => x.Patient.Email)
            .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Patient.Email))
            .WithMessage("El correo electrónico no es válido.");

        RuleFor(x => x.Patient.IdentityNumber)
            .MaximumLength(20).WithMessage("El número de identidad no debe exceder 20 caracteres.");

        RuleFor(x => x.Patient.DateOfBirth)
            .LessThan(DateTime.UtcNow).When(x => x.Patient.DateOfBirth.HasValue)
            .WithMessage("La fecha de nacimiento debe ser anterior a hoy.");
    }
}