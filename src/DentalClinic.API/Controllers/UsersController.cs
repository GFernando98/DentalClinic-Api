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
public class UsersController(UserManager<ApplicationUser> userManager) : ControllerBase
{
    [HttpGet("GetAll")]
    public async Task<ActionResult<Result<List<UserInfoDto>>>> GetAll()
    {
        var users = await userManager.Users
            .Where(u => u.IsActive)
            .OrderBy(u => u.FirstName)
            .ToListAsync();

        var usersDto = new List<UserInfoDto>();
        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
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

    [HttpPut("ToggleActive/{id}/toggle-active")]
    public async Task<ActionResult<Result<bool>>> ToggleActive(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        user.IsActive = !user.IsActive;
        if (!user.IsActive)
        {
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
        }

        await userManager.UpdateAsync(user);
        return Ok(Result<bool>.Success(true,
            user.IsActive ? "Usuario activado." : "Usuario desactivado."));
    }

    [HttpPut("UpdateRoles/{id}/roles")]
    public async Task<ActionResult<Result<bool>>> UpdateRoles(string id, [FromBody] List<string> roles)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var currentRoles = await userManager.GetRolesAsync(user);
        await userManager.RemoveFromRolesAsync(user, currentRoles);
        await userManager.AddToRolesAsync(user, roles);

        return Ok(Result<bool>.Success(true, "Roles actualizados."));
    }

    [HttpPost("Create")]
    public async Task<ActionResult<Result<UserInfoDto>>> Create([FromBody] CreateUserDto dto)
    {
        
        var existingUser = await userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            return BadRequest(Result<UserInfoDto>.Failure("El email ya está registrado."));
        
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            EmailConfirmed = true, 
            IsActive = true
        };

        var result = await userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest(Result<UserInfoDto>.Failure($"Error al crear usuario: {errors}"));
        }
        
        if (dto.Roles != null && dto.Roles.Any())
        {
            await userManager.AddToRolesAsync(user, dto.Roles);
        }
        
        var assignedRoles = await userManager.GetRolesAsync(user);

        var userDto = new UserInfoDto
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.FullName,
            Roles = assignedRoles.ToList()
        };

        return Ok(Result<UserInfoDto>.Success(userDto, "Usuario creado exitosamente."));
    }
}