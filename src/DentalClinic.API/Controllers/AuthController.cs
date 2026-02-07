using System.Security.Claims;
using DentalClinic.Application.Common.Interfaces;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Auth.Commands;
using DentalClinic.Application.Features.Auth.DTOs;
using DentalClinic.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DentalClinic.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUser;

    public AuthController(IMediator mediator, UserManager<ApplicationUser> userManager, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _userManager = userManager;
        _currentUser = currentUser;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<AuthResponseDto>>> Login([FromBody] LoginDto request)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : Unauthorized(result);
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<AuthResponseDto>>> RefreshToken([FromBody] RefreshTokenDto request)
    {
        var command = new RefreshTokenCommand(request.AccessToken, request.RefreshToken);
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : Unauthorized(result);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<Result<bool>>> Logout()
    {
        var userId = _currentUser.UserId;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var command = new LogoutCommand(userId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("register")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<Result<UserInfoDto>>> Register([FromBody] RegisterUserDto request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            EmailConfirmed = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUser.UserId
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            return BadRequest(Result<UserInfoDto>.Failure(
                createResult.Errors.Select(e => e.Description).ToList()));
        }

        // Validate role
        string[] validRoles = { "Admin", "Doctor", "Receptionist", "Assistant" };
        if (!validRoles.Contains(request.Role))
            return BadRequest(Result<UserInfoDto>.Failure($"Rol inválido. Roles válidos: {string.Join(", ", validRoles)}"));

        await _userManager.AddToRoleAsync(user, request.Role);

        var roles = await _userManager.GetRolesAsync(user);
        var userInfo = new UserInfoDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.FullName,
            Roles = roles.ToList()
        };

        return Ok(Result<UserInfoDto>.Success(userInfo, "Usuario creado exitosamente."));
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult<Result<bool>>> ChangePassword([FromBody] ChangePasswordDto request)
    {
        var userId = _currentUser.UserId;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound();

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            return BadRequest(Result<bool>.Failure(
                result.Errors.Select(e => e.Description).ToList()));
        }

        return Ok(Result<bool>.Success(true, "Contraseña actualizada exitosamente."));
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<Result<UserInfoDto>>> GetCurrentUser()
    {
        var userId = _currentUser.UserId;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound();

        var roles = await _userManager.GetRolesAsync(user);
        var userInfo = new UserInfoDto
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.FullName,
            Roles = roles.ToList()
        };

        return Ok(Result<UserInfoDto>.Success(userInfo));
    }
}
