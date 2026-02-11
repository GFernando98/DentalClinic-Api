using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.TaxInformation.DTOs;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.TaxInformation.Queries;

public record GetAllTaxInformationQuery : IRequest<Result<IReadOnlyList<TaxInformationDto>>>;

public class GetAllTaxInformationQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetAllTaxInformationQuery, Result<IReadOnlyList<TaxInformationDto>>>
{
    public async Task<Result<IReadOnlyList<TaxInformationDto>>> Handle(
        GetAllTaxInformationQuery request, 
        CancellationToken ct)
    {
        var taxInfoList = await unitOfWork.TaxInformation.GetAllAsync(ct);

        var dtos = taxInfoList
            .OrderByDescending(t => t.IsActive)
            .ThenByDescending(t => t.ExpirationDate)
            .Select(t => new TaxInformationDto
            {
                Id = t.Id,
                CAI = t.CAI,
                InvoiceType = t.InvoiceType,
                RangeStart = t.RangeStart,
                RangeEnd = t.RangeEnd,
                CurrentNumber = t.CurrentNumber,
                AuthorizationDate = t.AuthorizationDate,
                ExpirationDate = t.ExpirationDate,
                IsActive = t.IsActive,
                IsExpired = t.IsExpired,
                IsExhausted = t.IsExhausted,
                CanGenerateInvoice = t.CanGenerateInvoice,
                RemainingInvoices = t.RemainingInvoices
            })
            .ToList()
            .AsReadOnly();

        return Result<IReadOnlyList<TaxInformationDto>>.Success(dtos);
    }
}