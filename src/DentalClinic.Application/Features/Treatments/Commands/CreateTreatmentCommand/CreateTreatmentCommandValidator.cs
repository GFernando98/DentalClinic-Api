using FluentValidation;

namespace DentalClinic.Application.Features.Treatments.Commands.CreateTreatmentCommand;

public class CreateTreatmentCommandValidator : AbstractValidator<CreateTreatmentCommand>
{
    public CreateTreatmentCommandValidator()
    {
        RuleFor(x => x.Treatment)
            .NotNull()
            .WithMessage("Los datos del tratamiento son requeridos.");

        RuleFor(x => x.Treatment.Code)
            .NotEmpty()
            .WithMessage("El código es requerido.")
            .MaximumLength(20)
            .WithMessage("El código no puede exceder 20 caracteres.")
            .Matches(@"^[A-Z0-9-]+$")
            .WithMessage("El código solo puede contener letras mayúsculas, números y guiones.");

        RuleFor(x => x.Treatment.Name)
            .NotEmpty()
            .WithMessage("El nombre es requerido.")
            .MaximumLength(200)
            .WithMessage("El nombre no puede exceder 200 caracteres.");

        RuleFor(x => x.Treatment.Description)
            .MaximumLength(1000)
            .WithMessage("La descripción no puede exceder 1000 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Treatment.Description));

        RuleFor(x => x.Treatment.CategoryId)
            .NotEmpty()
            .WithMessage("La categoría es requerida.");

        RuleFor(x => x.Treatment.DefaultPrice)
            .GreaterThan(0)
            .WithMessage("El precio debe ser mayor a cero.")
            .LessThan(1000000)
            .WithMessage("El precio no puede exceder 1,000,000.");

        RuleFor(x => x.Treatment.EstimatedDurationMinutes)
            .GreaterThan(0)
            .WithMessage("La duración estimada debe ser mayor a cero.")
            .LessThanOrEqualTo(480)
            .WithMessage("La duración estimada no puede exceder 8 horas (480 minutos).");
    }
}