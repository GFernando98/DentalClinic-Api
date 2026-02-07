using DentalClinic.Application.Features.Auth.DTOs;

namespace DentalClinic.Application.Common.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> RefreshTokenAsync(string accessToken, string refreshToken, CancellationToken cancellationToken = default);
    Task RevokeRefreshTokenAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);
}

public interface ICurrentUserService
{
    string? UserId { get; }
    string? Email { get; }
    string? FullName { get; }
    IEnumerable<string> Roles { get; }
    bool IsAuthenticated { get; }
}

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default);
}

public interface IWhatsAppService
{
    Task<bool> SendMessageAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);
    Task<bool> SendTemplateMessageAsync(string phoneNumber, string templateName, Dictionary<string, string> parameters, CancellationToken cancellationToken = default);
}

public interface IDateTimeService
{
    DateTime UtcNow { get; }
}
