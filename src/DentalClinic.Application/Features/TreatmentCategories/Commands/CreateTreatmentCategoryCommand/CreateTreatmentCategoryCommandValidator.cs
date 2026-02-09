using FluentValidation;

namespace DentalClinic.Application.Features.TreatmentCategories.Commands.CreateTreatmentCategoryCommand;

public class CreateTreatmentCategoryCommandValidator : AbstractValidator<CreateTreatmentCategoryCommand>
{
    public CreateTreatmentCategoryCommandValidator()
    {
        RuleFor(x => x.Category.Name)
            .NotEmpty().WithMessage("El nombre de la categoría es requerido.")
            .MaximumLength(100).WithMessage("El nombre no debe exceder 100 caracteres.");

        RuleFor(x => x.Category.Color)
            .MaximumLength(10).WithMessage("El color no debe exceder 10 caracteres.")
            .Matches(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$")
            .When(x => !string.IsNullOrWhiteSpace(x.Category.Color))
            .WithMessage("El color debe ser un código hexadecimal válido (ej: #FF5733).");

        RuleFor(x => x.Category.Description)
            .MaximumLength(500).WithMessage("La descripción no debe exceder 500 caracteres.");
    }
}