using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.TaxInformation.Commands.CreateTaxInformationCommand;
using DentalClinic.Application.Features.TaxInformation.Commands.DeactivateTaxInformationCommand;
using DentalClinic.Application.Features.TaxInformation.DTOs;
using DentalClinic.Application.Features.TaxInformation.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalClinic.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AdminOnly")]
public class TaxInformationController(IMediator mediator) : ControllerBase
{
    [HttpGet("GetAll")]
    public async Task<ActionResult<Result<IReadOnlyList<TaxInformationDto>>>> GetAll(CancellationToken ct)
    {
        var query = new GetAllTaxInformationQuery();
        var result = await mediator.Send(query, ct);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
    
    [HttpPost("Create")]
    public async Task<ActionResult<Result<TaxInformationDto>>> Create(
        [FromBody] CreateTaxInformationDto dto,
        CancellationToken ct)
    {
        var command = new CreateTaxInformationCommand(dto);
        var result = await mediator.Send(command, ct);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
    
    [HttpPut("Deactivate/{id}")]
    public async Task<ActionResult<Result<bool>>> Deactivate(Guid id, CancellationToken ct)
    {
        var command = new DeactivateTaxInformationCommand(id);
        var result = await mediator.Send(command, ct);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}