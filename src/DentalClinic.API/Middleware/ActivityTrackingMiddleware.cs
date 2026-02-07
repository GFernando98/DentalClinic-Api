using System.Security.Claims;
using DentalClinic.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DentalClinic.API.Middleware;

/// <summary>
/// Tracks user activity for inactivity-based session timeout.
/// Updates LastActivity on every authenticated request.
/// </summary>
public class ActivityTrackingMiddleware
{
    private readonly RequestDelegate _next;

    public ActivityTrackingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, UserManager<ApplicationUser> userManager)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    user.LastActivity = DateTime.UtcNow;
                    await userManager.UpdateAsync(user);
                }
            }
        }

        await _next(context);
    }
}
