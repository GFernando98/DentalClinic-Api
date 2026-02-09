using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Dashboard.DTOs;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Dashboard.Queries;

public record GetTreatmentsByCategoryQuery : IRequest<Result<IReadOnlyList<TreatmentsByCategoryDto>>>;

public class GetTreatmentsByCategoryHandler : IRequestHandler<GetTreatmentsByCategoryQuery, Result<IReadOnlyList<TreatmentsByCategoryDto>>>
{
    private readonly IUnitOfWork _uow;
    public GetTreatmentsByCategoryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<IReadOnlyList<TreatmentsByCategoryDto>>> Handle(GetTreatmentsByCategoryQuery request, CancellationToken ct)
    {
        var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

        var records = await _uow.TreatmentRecords.FindAsync(
            tr => tr.PerformedDate >= monthStart, ct);

        var categories = await _uow.TreatmentCategories.GetAllAsync(ct);
        var treatments = await _uow.Treatments.GetAllAsync(ct);

        var grouped = records
            .GroupBy(r =>
            {
                var treatment = treatments.FirstOrDefault(t => t.Id == r.TreatmentId);
                var category = treatment != null
                    ? categories.FirstOrDefault(c => c.Id == treatment.CategoryId)
                    : null;
                return category;
            })
            .Where(g => g.Key != null)
            .Select(g => new TreatmentsByCategoryDto
            {
                Category = g.Key!.Name,
                Color = g.Key.Color ?? "#6B7280",
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .ToList();

        return Result<IReadOnlyList<TreatmentsByCategoryDto>>.Success(grouped.AsReadOnly());
    }
}