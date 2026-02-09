using DentalClinic.Application.Common.Interfaces;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Auth.DTOs;
using MediatR;

namespace DentalClinic.Application.Features.Auth.Commands.RefreshTokenCommand;

public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<Result<AuthResponseDto>>;

public class RefreshTokenCommandHandler(IAuthService authService)
    : IRequestHandler<RefreshTokenCommand, Result<AuthResponseDto>>
{
    public async Task<Result<AuthResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var result = await authService.RefreshTokenAsync(request.AccessToken, request.RefreshToken, cancellationToken);

        return result.Succeeded
            ? Result<AuthResponseDto>.Success(result)
            : Result<AuthResponseDto>.Failure(result.ErrorMessage ?? "Token refresh failed.");
    }
}
