using MediatR;
using QLine.Application.DTO;

namespace QLine.Application.Features.Reservations.Commands
{
    public sealed record RescheduleReservationCommand(
        Guid ReservationId,
        DateTimeOffset NewStartTime
    ) : IRequest<ReservationDto>;
}
