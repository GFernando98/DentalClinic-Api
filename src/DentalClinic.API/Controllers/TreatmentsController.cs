using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Treatments.Commands.CreateTreatmentCommand;
using DentalClinic.Application.Features.Treatments.Commands.UpdateTreatmentCommand;
using DentalClinic.Application.Features.Treatments.DTOs;
using DentalClinic.Application.Features.Treatments.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalClinic.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TreatmentsController(IMediator mediator) : ControllerBase
{
    [HttpGet("GetAll")]
    public async Task<ActionResult<Result<IReadOnlyList<TreatmentDto>>>> GetAll(CancellationToken ct)
        => Ok(await mediator.Send(new GetAllTreatmentsQuery(), ct));

    [HttpGet("GetById/{id:guid}")]
    public async Task<ActionResult<Result<TreatmentDto>>> GetById(Guid id, CancellationToken ct)
        => Ok(await mediator.Send(new GetTreatmentByIdQuery(id), ct));

    [HttpPost("Create")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<Result<TreatmentDto>>> Create([FromBody] CreateTreatmentDto dto, CancellationToken ct)
    {
        var result = await mediator.Send(new CreateTreatmentCommand(dto), ct);
        return result.Succeeded
            ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result)
            : BadRequest(result);
    }

    [HttpPut("Update/{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<Result<TreatmentDto>>> Update(Guid id, [FromBody] CreateTreatmentDto dto, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateTreatmentCommand(id, dto), ct);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}
