using MediatR;
using QLine.Application.DTO;

namespace QLine.Application.Features.Reservations.Commands
{
    public sealed record CreateReservationCommand(
        Guid ServicePointId,
        Guid ServiceId,
        DateTimeOffset StartTime
        ) : IRequest<ReservationDto>;

}
