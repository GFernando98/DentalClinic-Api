using System.Net.Http.Json;
using DentalClinic.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DentalClinic.Infrastructure.Services;

/// <summary>
/// WhatsApp service using Meta WhatsApp Business API.
/// Configure your WhatsApp Business Account and get the API token.
/// Alternative: Use Twilio WhatsApp API by changing the base URL and auth.
/// </summary>
public class WhatsAppService : IWhatsAppService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<WhatsAppService> _logger;

    public WhatsAppService(HttpClient httpClient, IConfiguration configuration, ILogger<WhatsAppService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> SendMessageAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
    {
        try
        {
            var phoneNumberId = _configuration["WhatsApp:PhoneNumberId"];
            var apiToken = _configuration["WhatsApp:ApiToken"];
            var apiVersion = _configuration["WhatsApp:ApiVersion"] ?? "v18.0";

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiToken);

            var payload = new
            {
                messaging_product = "whatsapp",
                to = NormalizePhoneNumber(phoneNumber),
                type = "text",
                text = new { body = message }
            };

            var response = await _httpClient.PostAsJsonAsync(
                $"https://graph.facebook.com/{apiVersion}/{phoneNumberId}/messages",
                payload,
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("WhatsApp message sent to {Phone}", phoneNumber);
                return true;
            }

            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("WhatsApp API error: {StatusCode} - {Error}", response.StatusCode, errorContent);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send WhatsApp message to {Phone}", phoneNumber);
            return false;
        }
    }

    public async Task<bool> SendTemplateMessageAsync(string phoneNumber, string templateName, Dictionary<string, string> parameters, CancellationToken cancellationToken = default)
    {
        try
        {
            var phoneNumberId = _configuration["WhatsApp:PhoneNumberId"];
            var apiToken = _configuration["WhatsApp:ApiToken"];
            var apiVersion = _configuration["WhatsApp:ApiVersion"] ?? "v18.0";
            var language = _configuration["WhatsApp:DefaultLanguage"] ?? "es";

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiToken);

            var components = new List<object>();
            if (parameters.Any())
            {
                components.Add(new
                {
                    type = "body",
                    parameters = parameters.Select(p => new { type = "text", text = p.Value }).ToArray()
                });
            }

            var payload = new
            {
                messaging_product = "whatsapp",
                to = NormalizePhoneNumber(phoneNumber),
                type = "template",
                template = new
                {
                    name = templateName,
                    language = new { code = language },
                    components
                }
            };

            var response = await _httpClient.PostAsJsonAsync(
                $"https://graph.facebook.com/{apiVersion}/{phoneNumberId}/messages",
                payload,
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("WhatsApp template message '{Template}' sent to {Phone}", templateName, phoneNumber);
                return true;
            }

            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("WhatsApp API error: {StatusCode} - {Error}", response.StatusCode, errorContent);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send WhatsApp template to {Phone}", phoneNumber);
            return false;
        }
    }

    /// <summary>
    /// Normalize Honduras phone numbers to international format.
    /// </summary>
    private static string NormalizePhoneNumber(string phone)
    {
        var cleaned = new string(phone.Where(char.IsDigit).ToArray());

        // If it's a Honduras number without country code
        if (cleaned.Length == 8 && (cleaned.StartsWith("9") || cleaned.StartsWith("3") || cleaned.StartsWith("2")))
            cleaned = "504" + cleaned;

        return cleaned;
    }
}
