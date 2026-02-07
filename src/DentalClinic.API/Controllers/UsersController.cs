using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Auth.DTOs;
using DentalClinic.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentalClinic.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AdminOnly")]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<Result<List<UserInfoDto>>>> GetAll()
    {
        var users = await _userManager.Users
            .Where(u => u.IsActive)
            .OrderBy(u => u.FirstName)
            .ToListAsync();

        var usersDto = new List<UserInfoDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            usersDto.Add(new UserInfoDto
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                Roles = roles.ToList()
            });
        }

        return Ok(Result<List<UserInfoDto>>.Success(usersDto));
    }

    [HttpPut("{id}/toggle-active")]
    public async Task<ActionResult<Result<bool>>> ToggleActive(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        user.IsActive = !user.IsActive;
        if (!user.IsActive)
        {
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
        }

        await _userManager.UpdateAsync(user);
        return Ok(Result<bool>.Success(true,
            user.IsActive ? "Usuario activado." : "Usuario desactivado."));
    }

    [HttpPut("{id}/roles")]
    public async Task<ActionResult<Result<bool>>> UpdateRoles(string id, [FromBody] List<string> roles)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRolesAsync(user, roles);

        return Ok(Result<bool>.Success(true, "Roles actualizados."));
    }
}
