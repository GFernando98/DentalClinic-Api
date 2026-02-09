using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Doctors.Commands.CreateDoctorCommand;
using DentalClinic.Application.Features.Doctors.Commands.UpdateDoctorCommand;
using DentalClinic.Application.Features.Doctors.DTOs;
using DentalClinic.Application.Features.Doctors.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalClinic.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "ReceptionistOrAbove")]
public class DoctorsController(IMediator mediator) : ControllerBase
{
    [HttpGet("GetAll")]
    public async Task<ActionResult<Result<IReadOnlyList<DoctorDto>>>> GetAll(CancellationToken ct)
        => Ok(await mediator.Send(new GetAllDoctorsQuery(), ct));

    [HttpGet("GetById/{id:guid}")]
    public async Task<ActionResult<Result<DoctorDto>>> GetById(Guid id, CancellationToken ct)
        => Ok(await mediator.Send(new GetDoctorByIdQuery(id), ct));

    [HttpPost("Create")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<Result<DoctorDto>>> Create([FromBody] CreateDoctorDto dto, CancellationToken ct)
    {
        var result = await mediator.Send(new CreateDoctorCommand(dto), ct);
        return result.Succeeded
            ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result)
            : BadRequest(result);
    }

    [HttpPut("Update/{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<Result<DoctorDto>>> Update(Guid id, [FromBody] CreateDoctorDto dto, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateDoctorCommand(id, dto), ct);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}
