using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.ClinicInformation.Commads.UpdateClinicInformationCommand;
using DentalClinic.Application.Features.ClinicInformation.DTOs;
using DentalClinic.Application.Features.ClinicInformation.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalClinic.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClinicInformationController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<Result<ClinicInformationDto>>> GetClinicInformation(CancellationToken ct)
    {
        var query = new GetClinicInformationQuery();
        var result = await mediator.Send(query, ct);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
    
    [HttpPut]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<Result<ClinicInformationDto>>> UpdateClinicInformation(
        [FromBody] UpdateClinicInformationDto dto,
        CancellationToken ct)
    {
        var command = new UpdateClinicInformationCommand(dto);
        var result = await mediator.Send(command, ct);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}