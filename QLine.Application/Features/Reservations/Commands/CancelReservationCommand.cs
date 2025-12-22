using MediatR;

namespace QLine.Application.Features.Reservations.Commands
{
    public sealed record CancelReservationCommand(Guid ReservationId) : IRequest;
}
