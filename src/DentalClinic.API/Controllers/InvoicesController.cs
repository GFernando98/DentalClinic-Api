using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Invoices.DTOs;
using DentalClinic.Application.Features.Invoices.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalClinic.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "DoctorOrAdmin")]
public class InvoicesController(IMediator mediator) : ControllerBase
{
    [HttpGet("Preview/{odontogramId}")]
    public async Task<ActionResult<Result<InvoicePreviewDto>>> GetInvoicePreview(
        Guid odontogramId, 
        CancellationToken ct)
    {
        var query = new GetInvoicePreviewQuery(odontogramId);
        var result = await mediator.Send(query, ct);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}