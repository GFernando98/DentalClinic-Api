using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Interfaces;
using DentalClinic.Application.Common.Models;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Patients.Commands.DeletePatientCommand;

public record DeletePatientCommand(Guid Id) : IRequest<Result<bool>>;

public class DeletePatientCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<DeletePatientCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeletePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = await unitOfWork.Patients.GetByIdAsync(request.Id, cancellationToken)
                      ?? throw new NotFoundException(nameof(Patient), request.Id);

        patient.IsDeleted = true;
        patient.DeletedAt = DateTime.UtcNow;
        patient.DeletedBy = currentUser.UserId;

        await unitOfWork.Patients.UpdateAsync(patient, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "Paciente eliminado.");
    }
}
