using MediatR;
using QLine.Application.Abstractions;
using QLine.Domain;
using QLine.Domain.Abstractions;

namespace QLine.Application.Features.Reservations.Commands
{
    public sealed class CancelReservationCommandHandler : IRequestHandler<CancelReservationCommand>
    {
        private readonly IReservationRepository _reservations;
        private readonly ICurrentUser _currentUser;

        public CancelReservationCommandHandler(
            IReservationRepository reservations,
            ICurrentUser currentUser)
        {
            _reservations = reservations;
            _currentUser = currentUser;
        }

        public async Task Handle(CancelReservationCommand request, CancellationToken ct)
        {
            var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();

            var reservation = await _reservations.GetByIdAsync(request.ReservationId, ct)
                ?? throw new DomainException("Reservation not found.");

            if (reservation.UserId != userId)
                throw new UnauthorizedAccessException();

            if (reservation.Status == Domain.Enums.ReservationStatus.Cancelled)
                return;

            reservation.Cancel();
            await _reservations.UpdateAsync(reservation, ct);
        }
    }
}
