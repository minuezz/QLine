using MediatR;
using QLine.Application.Abstractions;
using QLine.Domain;
using QLine.Domain.Abstractions;
using QLine.Domain.Enums;

namespace QLine.Application.Features.Reservations.Commands
{
    public sealed class CancelReservationCommandHandler : IRequestHandler<CancelReservationCommand>
    {
        private readonly IReservationRepository _reservations;
        private readonly IQueueEntryRepository _queueEntries;
        private readonly ICurrentUser _currentUser;
        private readonly IRealtimeNotifier _realtime;

        public CancelReservationCommandHandler(
            IReservationRepository reservations,
            IQueueEntryRepository queueEntries,
            ICurrentUser currentUser,
            IRealtimeNotifier realtime)
        {
            _reservations = reservations;
            _queueEntries = queueEntries;
            _currentUser = currentUser;
            _realtime = realtime;
        }

        public async Task Handle(CancelReservationCommand request, CancellationToken ct)
        {
            var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();

            var reservation = await _reservations.GetByIdAsync(request.ReservationId, ct)
                ?? throw new DomainException("Reservation not found.");

            if (reservation.UserId != userId)
                throw new UnauthorizedAccessException();

            if (reservation.Status != ReservationStatus.Active && reservation.Status != ReservationStatus.Waiting)
                throw new DomainException("Reservation is not active.");

            var queueEntry = await _queueEntries.GetByReservationAsync(reservation.Id, ct);
            
            if (queueEntry is not null)
            {
                await _queueEntries.DeleteAsync(queueEntry, ct);

                await _realtime.QueueUpdated(reservation.ServicePointId, ct);
            }

            reservation.Cancel();
            await _reservations.UpdateAsync(reservation, ct);
            await _realtime.UserReservationsUpdated(userId, ct);
        }
    }
}
