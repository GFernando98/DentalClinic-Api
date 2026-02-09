using FluentValidation;

namespace DentalClinic.Application.Features.TreatmentCategories.Commands.ToggleTreatmentCategoryCommand;

public class ToggleTreatmentCategoryCommandValidator : AbstractValidator<ToggleTreatmentCategoryCommand>
{
    public ToggleTreatmentCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID de la categoría es requerido.");
    }
}