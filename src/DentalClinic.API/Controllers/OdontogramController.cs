using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Odontogram.Commands.AddSurfaceRecordCommand;
using DentalClinic.Application.Features.Odontogram.Commands.AddTreatmentRecordCommand;
using DentalClinic.Application.Features.Odontogram.Commands.CreateOdontogramCommand;
using DentalClinic.Application.Features.Odontogram.Commands.UpdateToothCommand;
using DentalClinic.Application.Features.Odontogram.DTOs;
using DentalClinic.Application.Features.Odontogram.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalClinic.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "DoctorOrAdmin")]
public class OdontogramController(IMediator mediator) : ControllerBase
{
    [HttpGet("GetByPatient/{patientId:guid}")]
    public async Task<ActionResult<Result<IReadOnlyList<OdontogramDto>>>> GetByPatient(Guid patientId, CancellationToken ct)
        => Ok(await mediator.Send(new GetPatientOdontogramsQuery(patientId), ct));

    [HttpGet("GetById/{id:guid}")]
    public async Task<ActionResult<Result<OdontogramDto>>> GetById(Guid id, CancellationToken ct)
        => Ok(await mediator.Send(new GetOdontogramByIdQuery(id), ct));

    [HttpPost("Create")]
    public async Task<ActionResult<Result<OdontogramDto>>> Create([FromBody] CreateOdontogramDto dto, CancellationToken ct)
    {
        var result = await mediator.Send(new CreateOdontogramCommand(dto), ct);
        return result.Succeeded
            ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result)
            : BadRequest(result);
    }

    [HttpPut("UpdateTooth/{toothRecordId:guid}")]
    public async Task<ActionResult<Result<ToothRecordDto>>> UpdateTooth(
        Guid toothRecordId, [FromBody] UpdateToothDto dto, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateToothCommand(toothRecordId, dto), ct);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPost("AddSurface/{toothRecordId:guid}")]
    public async Task<ActionResult<Result<ToothSurfaceDto>>> AddSurface(
        Guid toothRecordId, [FromBody] AddSurfaceDto dto, CancellationToken ct)
    {
        var result = await mediator.Send(new AddSurfaceRecordCommand(toothRecordId, dto), ct);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPost("AddTreatment/{toothRecordId:guid}")]
    public async Task<ActionResult<Result<TreatmentRecordDto>>> AddTreatment(
        Guid toothRecordId, [FromBody] AddTreatmentRecordDto dto, CancellationToken ct)
    {
        var result = await mediator.Send(new AddTreatmentRecordCommand(toothRecordId, dto), ct);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpGet("GetToothTreatments/{toothRecordId:guid}")]
    public async Task<ActionResult<Result<IReadOnlyList<TreatmentRecordDto>>>> GetToothTreatments(
        Guid toothRecordId, CancellationToken ct)
        => Ok(await mediator.Send(new GetToothTreatmentsQuery(toothRecordId), ct));
}
