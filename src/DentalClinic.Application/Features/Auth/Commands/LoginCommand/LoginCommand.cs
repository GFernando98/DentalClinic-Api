using DentalClinic.Application.Common.Interfaces;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Auth.DTOs;
using MediatR;

namespace DentalClinic.Application.Features.Auth.Commands.LoginCommand;

public record LoginCommand(string Email, string Password) : IRequest<Result<AuthResponseDto>>;

public class LoginCommandHandler(IAuthService authService) : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
{
    public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(request.Email, request.Password, cancellationToken);

        return result.Succeeded
            ? Result<AuthResponseDto>.Success(result)
            : Result<AuthResponseDto>.Failure(result.ErrorMessage ?? "Login failed.");
    }
}