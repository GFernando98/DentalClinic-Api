using DentalClinic.Application.Common.Interfaces;
using DentalClinic.Application.Common.Models;
using MediatR;

namespace DentalClinic.Application.Features.Auth.Commands.LogoutCommand;

public record LogoutCommand(string UserId) : IRequest<Result<bool>>;

public class LogoutCommandHandler(IAuthService authService) : IRequestHandler<LogoutCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await authService.RevokeRefreshTokenAsync(request.UserId, cancellationToken);
        return Result<bool>.Success(true, "Logged out successfully.");
    }
}
