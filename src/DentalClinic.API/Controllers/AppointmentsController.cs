using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Appointments.Commands.CreateAppointmentCommand;
using DentalClinic.Application.Features.Appointments.Commands.DeleteAppointmentCommand;
using DentalClinic.Application.Features.Appointments.Commands.UpdateAppointmentCommand;
using DentalClinic.Application.Features.Appointments.Commands.UpdateAppointmentStatusCommand;
using DentalClinic.Application.Features.Appointments.DTOs;
using DentalClinic.Application.Features.Appointments.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalClinic.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "ReceptionistOrAbove")]
public class AppointmentsController(IMediator mediator) : ControllerBase
{
    [HttpGet("GetAll")]
    public async Task<ActionResult<Result<IReadOnlyList<AppointmentDto>>>> GetAll(
        [FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] Guid? doctorId, CancellationToken ct)
        => Ok(await mediator.Send(new GetAppointmentsQuery(from, to, doctorId), ct));

    [HttpGet("GetById/{id:guid}")]
    public async Task<ActionResult<Result<AppointmentDto>>> GetById(Guid id, CancellationToken ct)
        => Ok(await mediator.Send(new GetAppointmentByIdQuery(id), ct));

    [HttpGet("GetByPatient/{patientId:guid}")]
    public async Task<ActionResult<Result<IReadOnlyList<AppointmentDto>>>> GetByPatient(Guid patientId, CancellationToken ct)
        => Ok(await mediator.Send(new GetPatientAppointmentsQuery(patientId), ct));

    [HttpGet("GetToday")]
    public async Task<ActionResult<Result<IReadOnlyList<AppointmentDto>>>> GetToday(
        [FromQuery] Guid? doctorId, CancellationToken ct)
        => Ok(await mediator.Send(new GetTodayAppointmentsQuery(doctorId), ct));

    [HttpPost("Create")]
    public async Task<ActionResult<Result<AppointmentDto>>> Create([FromBody] CreateAppointmentDto dto, CancellationToken ct)
    {
        var result = await mediator.Send(new CreateAppointmentCommand(dto), ct);
        return result.Succeeded
            ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result)
            : BadRequest(result);
    }

    [HttpPut("Update/{id:guid}")]
    public async Task<ActionResult<Result<AppointmentDto>>> Update(Guid id, [FromBody] CreateAppointmentDto dto, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateAppointmentCommand(id, dto), ct);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPut("UpdateStatus/{id:guid}/status")]
    public async Task<ActionResult<Result<AppointmentDto>>> UpdateStatus(
        Guid id, [FromBody] UpdateAppointmentStatusDto dto, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateAppointmentStatusCommand(id, dto), ct);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("Delete/{id:guid}")]
    public async Task<ActionResult<Result<bool>>> Delete(Guid id, CancellationToken ct)
        => Ok(await mediator.Send(new DeleteAppointmentCommand(id), ct));
}
