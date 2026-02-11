using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Invoices.Commands.CancelInvoiceCommand;
using DentalClinic.Application.Features.Invoices.Commands.CreateInvoiceCommand;
using DentalClinic.Application.Features.Invoices.Commands.RegisterPaymentCommand;
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
    
    [HttpPost("Create")]
    public async Task<ActionResult<Result<InvoiceDto>>> CreateInvoice(
        [FromBody] CreateInvoiceDto dto,
        CancellationToken ct)
    {
        var command = new CreateInvoiceCommand(dto);
        var result = await mediator.Send(command, ct);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
    
    [HttpGet("GetById/{invoiceId}")]
    public async Task<ActionResult<Result<InvoiceDto>>> GetById(Guid invoiceId, CancellationToken ct)
    {
        var query = new GetInvoiceByIdQuery(invoiceId);
        var result = await mediator.Send(query, ct);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpGet("GetByPatient/{patientId}")]
    public async Task<ActionResult<Result<IReadOnlyList<InvoiceDto>>>> GetByPatient(
        Guid patientId, 
        CancellationToken ct)
    {
        var query = new GetPatientInvoicesQuery(patientId);
        var result = await mediator.Send(query, ct);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
    
    [HttpPost("RegisterPayment")]
    public async Task<ActionResult<Result<PaymentDto>>> RegisterPayment(
        [FromBody] CreatePaymentDto dto,
        CancellationToken ct)
    {
        var command = new RegisterPaymentCommand(dto);
        var result = await mediator.Send(command, ct);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
    
    [HttpPut("Cancel/{invoiceId}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<Result<bool>>> CancelInvoice(
        Guid invoiceId,
        [FromBody] string reason,
        CancellationToken ct)
    {
        var command = new CancelInvoiceCommand(invoiceId, reason);
        var result = await mediator.Send(command, ct);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
    
    [HttpGet("Revenue")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<Result<RevenueReportDto>>> GetRevenueReport(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        CancellationToken ct)
    {
        var query = new GetRevenueReportQuery(startDate, endDate);
        var result = await mediator.Send(query, ct);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}