using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Appointments.Commands;
using DentalClinic.Application.Features.Appointments.DTOs;
using DentalClinic.Application.Features.Appointments.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalClinic.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "ReceptionistOrAbove")]
public class AppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AppointmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<Result<IReadOnlyList<AppointmentDto>>>> GetAll(
        [FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] Guid? doctorId, CancellationToken ct)
        => Ok(await _mediator.Send(new GetAppointmentsQuery(from, to, doctorId), ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Result<AppointmentDto>>> GetById(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetAppointmentByIdQuery(id), ct));

    [HttpGet("patient/{patientId:guid}")]
    public async Task<ActionResult<Result<IReadOnlyList<AppointmentDto>>>> GetByPatient(Guid patientId, CancellationToken ct)
        => Ok(await _mediator.Send(new GetPatientAppointmentsQuery(patientId), ct));

    [HttpGet("today")]
    public async Task<ActionResult<Result<IReadOnlyList<AppointmentDto>>>> GetToday(
        [FromQuery] Guid? doctorId, CancellationToken ct)
        => Ok(await _mediator.Send(new GetTodayAppointmentsQuery(doctorId), ct));

    [HttpPost]
    public async Task<ActionResult<Result<AppointmentDto>>> Create([FromBody] CreateAppointmentDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateAppointmentCommand(dto), ct);
        return result.Succeeded
            ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result)
            : BadRequest(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Result<AppointmentDto>>> Update(Guid id, [FromBody] CreateAppointmentDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateAppointmentCommand(id, dto), ct);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:guid}/status")]
    public async Task<ActionResult<Result<AppointmentDto>>> UpdateStatus(
        Guid id, [FromBody] UpdateAppointmentStatusDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateAppointmentStatusCommand(id, dto), ct);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<Result<bool>>> Delete(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new DeleteAppointmentCommand(id), ct));
}
