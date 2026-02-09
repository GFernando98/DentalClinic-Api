using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Odontogram.DTOs;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Odontogram.Queries;

public record GetOdontogramByIdQuery(Guid Id) : IRequest<Result<OdontogramDto>>;

public class GetOdontogramByIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetOdontogramByIdQuery, Result<OdontogramDto>>
{
    public async Task<Result<OdontogramDto>> Handle(GetOdontogramByIdQuery request, CancellationToken cancellationToken)
    {
        var odontogram = await unitOfWork.Odontograms.GetByIdAsync(request.Id, cancellationToken)
                         ?? throw new NotFoundException("Odontograma", request.Id);
        
        var teeth = await unitOfWork.ToothRecords.FindAsync(
            t => t.OdontogramId == request.Id, cancellationToken);

        var teethDtos = new List<ToothRecordDto>();
        foreach (var tooth in teeth)
        {
            var surfaces = await unitOfWork.ToothSurfaceRecords.FindAsync(
                s => s.ToothRecordId == tooth.Id, cancellationToken);

            teethDtos.Add(new ToothRecordDto
            {
                Id = tooth.Id,
                ToothNumber = tooth.ToothNumber,
                ToothType = tooth.ToothType,
                Condition = tooth.Condition,
                IsPresent = tooth.IsPresent,
                Notes = tooth.Notes,
                Surfaces = surfaces.Select(s => new ToothSurfaceDto
                {
                    Id = s.Id, Surface = s.Surface,
                    Condition = s.Condition, Notes = s.Notes
                }).ToList()
            });
        }

        return Result<OdontogramDto>.Success(new OdontogramDto
        {
            Id = odontogram.Id,
            PatientId = odontogram.PatientId,
            ExaminationDate = odontogram.ExaminationDate,
            Notes = odontogram.Notes,
            DoctorId = odontogram.DoctorId,
            TeethRecords = teethDtos.OrderBy(t => t.ToothNumber).ToList()
        });
    }
}