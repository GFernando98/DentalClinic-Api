using DentalClinic.Application.Common.Interfaces;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Auth.DTOs;
using MediatR;

namespace DentalClinic.Application.Features.Auth.Commands;

// ─── LOGIN ────────────────────────────────────────────────────────────
public record LoginCommand(string Email, string Password) : IRequest<Result<AuthResponseDto>>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
{
    private readonly IAuthService _authService;

    public LoginCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request.Email, request.Password, cancellationToken);

        return result.Succeeded
            ? Result<AuthResponseDto>.Success(result)
            : Result<AuthResponseDto>.Failure(result.ErrorMessage ?? "Login failed.");
    }
}

// ─── REFRESH TOKEN ────────────────────────────────────────────────────
public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<Result<AuthResponseDto>>;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponseDto>>
{
    private readonly IAuthService _authService;

    public RefreshTokenCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result<AuthResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.RefreshTokenAsync(request.AccessToken, request.RefreshToken, cancellationToken);

        return result.Succeeded
            ? Result<AuthResponseDto>.Success(result)
            : Result<AuthResponseDto>.Failure(result.ErrorMessage ?? "Token refresh failed.");
    }
}

// ─── LOGOUT (Revoke Refresh Token) ───────────────────────────────────
public record LogoutCommand(string UserId) : IRequest<Result<bool>>;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<bool>>
{
    private readonly IAuthService _authService;

    public LogoutCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await _authService.RevokeRefreshTokenAsync(request.UserId, cancellationToken);
        return Result<bool>.Success(true, "Logged out successfully.");
    }
}
