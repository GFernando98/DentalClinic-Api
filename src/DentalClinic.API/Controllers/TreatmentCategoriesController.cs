using DentalClinic.Application.Features.TreatmentCategories.Commands;
using DentalClinic.Application.Features.TreatmentCategories.Commands.CreateTreatmentCategoryCommand;
using DentalClinic.Application.Features.TreatmentCategories.Commands.ToggleTreatmentCategoryCommand;
using DentalClinic.Application.Features.TreatmentCategories.Commands.UpdateTreatmentCategoryCommand;
using DentalClinic.Application.Features.TreatmentCategories.DTOs;
using DentalClinic.Application.Features.TreatmentCategories.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalClinic.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize()]
public class TreatmentCategoriesController(IMediator mediator) : ControllerBase
{
    [HttpGet("GetAll")]
    [Authorize]
    public async Task<ActionResult> GetAll(CancellationToken ct)
        => Ok(await mediator.Send(new GetAllTreatmentCategoriesQuery(), ct));

    [HttpGet("GetById/{id:guid}")]
    [Authorize]
    public async Task<ActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await mediator.Send(new GetTreatmentCategoryByIdQuery(id), ct));

    [HttpPost("Create")]
    public async Task<ActionResult> Create([FromBody] CreateTreatmentCategoryDto dto, CancellationToken ct)
    {
        var result = await mediator.Send(new CreateTreatmentCategoryCommand(dto), ct);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPut("Update/{id:guid}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] CreateTreatmentCategoryDto dto, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateTreatmentCategoryCommand(id, dto), ct);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPut("ToggleActive/{id:guid}")]
    public async Task<ActionResult> ToggleActive(Guid id, CancellationToken ct)
        => Ok(await mediator.Send(new ToggleTreatmentCategoryCommand(id), ct));
}