using AutoMapper;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Odontogram.DTOs;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Odontogram.Queries;

public record GetPatientOdontogramsQuery(Guid PatientId) : IRequest<Result<IReadOnlyList<OdontogramDto>>>;

public class GetPatientOdontogramsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetPatientOdontogramsQuery, Result<IReadOnlyList<OdontogramDto>>>
{
    public async Task<Result<IReadOnlyList<OdontogramDto>>> Handle(GetPatientOdontogramsQuery request, CancellationToken cancellationToken)
    {
        var odontograms = await unitOfWork.Odontograms.FindWithIncludeAsync(
            o => o.PatientId == request.PatientId,
            cancellationToken,
            o => o.TeethRecords, 
            o => o.Patient,  
            o => o.Doctor
        );
        
        var allToothIds = odontograms
            .SelectMany(o => o.TeethRecords)
            .Select(t => t.Id)
            .ToList();
        
        var allSurfaces = await unitOfWork.ToothSurfaceRecords.FindAsync(
            s => allToothIds.Contains(s.ToothRecordId),
            cancellationToken
        );
        
        var dtos = odontograms.Select(o => new OdontogramDto
        {
            Id = o.Id,
            PatientId = o.PatientId,
            PatientName = o.Patient?.FullName ?? string.Empty,
            ExaminationDate = o.ExaminationDate,
            Notes = o.Notes,
            DoctorId = o.DoctorId,
            DoctorName = o.Doctor?.FullName ?? string.Empty,
            TeethRecords = o.TeethRecords
                .OrderBy(t => t.ToothNumber)
                .Select(tooth => new ToothRecordDto
                {
                    Id = tooth.Id,
                    ToothNumber = tooth.ToothNumber,
                    ToothType = tooth.ToothType,
                    Condition = tooth.Condition,
                    IsPresent = tooth.IsPresent,
                    Notes = tooth.Notes,
                    Surfaces = allSurfaces
                        .Where(s => s.ToothRecordId == tooth.Id)
                        .Select(s => new ToothSurfaceDto
                        {
                            Id = s.Id,
                            Surface = s.Surface,
                            Condition = s.Condition,
                            Notes = s.Notes
                        }).ToList()
                }).ToList()
        }).ToList().AsReadOnly();

        return Result<IReadOnlyList<OdontogramDto>>.Success(dtos);
    }
}