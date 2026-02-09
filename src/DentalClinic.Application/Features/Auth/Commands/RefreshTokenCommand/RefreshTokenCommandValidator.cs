using FluentValidation;

namespace DentalClinic.Application.Features.Auth.Commands.RefreshTokenCommand;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty().WithMessage("El token de acceso es requerido.");

        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("El refresh token es requerido.");
    }
}