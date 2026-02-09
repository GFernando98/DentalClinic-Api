using FluentValidation;

namespace DentalClinic.Application.Features.Patients.Commands.UpdatePatientCommand;

public class UpdatePatientCommandValidator : AbstractValidator<UpdatePatientCommand>
{
    public UpdatePatientCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("El ID del paciente es requerido.");

        RuleFor(x => x.Patient.FirstName)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(100);

        RuleFor(x => x.Patient.LastName)
            .NotEmpty().WithMessage("El apellido es requerido.")
            .MaximumLength(100);

        RuleFor(x => x.Patient.Email)
            .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Patient.Email))
            .WithMessage("El correo electrónico no es válido.");
    }
}