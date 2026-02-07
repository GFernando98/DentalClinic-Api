using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Treatments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalClinic.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TreatmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TreatmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<Result<IReadOnlyList<TreatmentDto>>>> GetAll(CancellationToken ct)
        => Ok(await _mediator.Send(new GetAllTreatmentsQuery(), ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Result<TreatmentDto>>> GetById(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetTreatmentByIdQuery(id), ct));

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<Result<TreatmentDto>>> Create([FromBody] CreateTreatmentDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateTreatmentCommand(dto), ct);
        return result.Succeeded
            ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result)
            : BadRequest(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<Result<TreatmentDto>>> Update(Guid id, [FromBody] CreateTreatmentDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateTreatmentCommand(id, dto), ct);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}
