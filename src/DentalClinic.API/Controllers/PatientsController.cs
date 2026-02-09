using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Patients.Commands.CreatePatientCommand;
using DentalClinic.Application.Features.Patients.Commands.DeletePatientCommand;
using DentalClinic.Application.Features.Patients.Commands.UpdatePatientCommand;
using DentalClinic.Application.Features.Patients.DTOs;
using DentalClinic.Application.Features.Patients.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalClinic.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "ReceptionistOrAbove")]
public class PatientsController(IMediator mediator) : ControllerBase
{
    [HttpGet("GetAll")]
    public async Task<ActionResult<Result<IReadOnlyList<PatientDto>>>> GetAll(CancellationToken ct)
        => Ok(await mediator.Send(new GetAllPatientsQuery(), ct));

    [HttpGet("GetById/{id:guid}")]
    public async Task<ActionResult<Result<PatientDto>>> GetById(Guid id, CancellationToken ct)
        => Ok(await mediator.Send(new GetPatientByIdQuery(id), ct));

    [HttpGet("Search")]
    public async Task<ActionResult<Result<IReadOnlyList<PatientDto>>>> Search(
        [FromQuery] string? name, [FromQuery] string? identityNumber, CancellationToken ct)
        => Ok(await mediator.Send(new SearchPatientsQuery(name, identityNumber), ct));

    [HttpPost("Create")]
    public async Task<ActionResult<Result<PatientDto>>> Create([FromBody] CreatePatientDto dto, CancellationToken ct)
    {
        var result = await mediator.Send(new CreatePatientCommand(dto), ct);
        return result.Succeeded
            ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result)
            : BadRequest(result);
    }

    [HttpPut("Update/{id:guid}")]
    public async Task<ActionResult<Result<PatientDto>>> Update(Guid id, [FromBody] UpdatePatientDto dto, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdatePatientCommand(id, dto), ct);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("Delete/{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<Result<bool>>> Delete(Guid id, CancellationToken ct)
        => Ok(await mediator.Send(new DeletePatientCommand(id), ct));
}
