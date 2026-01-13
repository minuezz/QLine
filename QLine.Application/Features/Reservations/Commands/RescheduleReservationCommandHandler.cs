using MediatR;
using QLine.Application.Abstractions;
using QLine.Application.DTO;
using QLine.Domain;
using QLine.Domain.Abstractions;
using QLine.Domain.Enums;

namespace QLine.Application.Features.Reservations.Commands
{
    public sealed class RescheduleReservationCommandHandler
        : IRequestHandler<RescheduleReservationCommand, ReservationDto>
    {
        private readonly IReservationRepository _reservations;
        private readonly IDateTimeProvider _clock;
        private readonly ICurrentUser _currentUser;
        private readonly IRealtimeNotifier _realtime;

        public RescheduleReservationCommandHandler(
            IReservationRepository reservations,
            IDateTimeProvider clock,
            ICurrentUser currentUser,
            IRealtimeNotifier realtime)
        {
            _reservations = reservations;
            _clock = clock;
            _currentUser = currentUser;
            _realtime = realtime;
        }

        public async Task<ReservationDto> Handle(RescheduleReservationCommand request, CancellationToken ct)
        {
            var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();

            var reservation = await _reservations.GetByIdAsync(request.ReservationId, ct)
                ?? throw new DomainException("Reservation not found.");

            if (reservation.UserId != userId)
                throw new UnauthorizedAccessException();

            if (reservation.Status != ReservationStatus.Active)
                throw new DomainException("Reservation is not active.");

            var safeStartTime = request.NewStartTime.ToUniversalTime();

            if (safeStartTime < _clock.UtcNow.AddMinutes(-1))
            {
                throw new DomainException("Cannot reschedule a reservation to the past.");
            }

            if (reservation.StartTime != safeStartTime)
            {
                var slotFree = await _reservations.IsSlotAvailableAsync(
                    reservation.ServicePointId,
                    safeStartTime,
                    ct,
                    reservation.Id);

                if (!slotFree)
                    throw new DomainException("The selected slot is already occupied.");
            }

            reservation.Reschedule(safeStartTime);
            await _reservations.UpdateAsync(reservation, ct);

            await _realtime.UserReservationsUpdated(reservation.UserId, ct);

            return new ReservationDto
            {
                Id = reservation.Id,
                ServicePointId = reservation.ServicePointId,
                ServiceId = reservation.ServiceId,
                UserId = reservation.UserId,
                StartTime = reservation.StartTime,
                Status = reservation.Status.ToString()
            };
        }
    }
}
