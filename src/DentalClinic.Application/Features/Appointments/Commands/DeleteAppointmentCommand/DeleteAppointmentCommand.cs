using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Interfaces;
using DentalClinic.Application.Common.Models;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;
using MediatR;

namespace DentalClinic.Application.Features.Appointments.Commands.DeleteAppointmentCommand;

public record DeleteAppointmentCommand(Guid Id) : IRequest<Result<bool>>;

public class DeleteAppointmentCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<DeleteAppointmentCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await unitOfWork.Appointments.GetByIdAsync(request.Id, cancellationToken)
                          ?? throw new NotFoundException(nameof(Appointment), request.Id);

        appointment.IsDeleted = true;
        appointment.DeletedAt = DateTime.UtcNow;
        appointment.DeletedBy = currentUser.UserId;

        await unitOfWork.Appointments.UpdateAsync(appointment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "Cita eliminada.");
    }
}