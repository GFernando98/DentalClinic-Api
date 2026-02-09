using DentalClinic.Application.Features.Dashboard.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalClinic.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController(IMediator mediator) : ControllerBase
{
    [HttpGet("GetStats")]
    public async Task<ActionResult> GetStats(CancellationToken ct)
        => Ok(await mediator.Send(new GetDashboardStatsQuery(), ct));

    [HttpGet("GetAppointmentsByDay")]
    public async Task<ActionResult> GetAppointmentsByDay(CancellationToken ct)
        => Ok(await mediator.Send(new GetAppointmentsByDayQuery(), ct));

    [HttpGet("GetTreatmentsByCategory")]
    public async Task<ActionResult> GetTreatmentsByCategory(CancellationToken ct)
        => Ok(await mediator.Send(new GetTreatmentsByCategoryQuery(), ct));

    [HttpGet("GetUpcomingAppointments")]
    public async Task<ActionResult> GetUpcomingAppointments([FromQuery] int limit = 5, CancellationToken ct = default)
        => Ok(await mediator.Send(new GetUpcomingAppointmentsQuery(limit), ct));
}