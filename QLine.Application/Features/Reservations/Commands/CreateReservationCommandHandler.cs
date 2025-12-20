using System;
using MediatR;
using QLine.Application.Abstractions;
using QLine.Application.DTO;
using QLine.Domain;
using QLine.Domain.Abstractions;
using QLine.Domain.Entities;

namespace QLine.Application.Features.Reservations.Commands
{
    public sealed class CreateReservationCommandHandler
        : IRequestHandler<CreateReservationCommand, ReservationDto>
    {
        private readonly IReservationRepository _reservations;
        private readonly IQueueEntryRepository _queueEntries;
        private readonly IDateTimeProvider _clock;
        private readonly IRealtimeNotifier _realtime;
        private readonly ICurrentUser _currentUser;

        public CreateReservationCommandHandler(
            IReservationRepository reservations,
            IQueueEntryRepository queueEntries,
            IDateTimeProvider clock,
            IRealtimeNotifier realtime,
            ICurrentUser currentUser)
        {
            _reservations = reservations;
            _queueEntries = queueEntries;
            _clock = clock;
            _realtime = realtime;
            _currentUser = currentUser;
        }

        public async Task<ReservationDto> Handle(CreateReservationCommand request, CancellationToken ct)
        {
            var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();

            var safeStartTime = request.StartTime.ToUniversalTime();

            var slotFree = await _reservations.IsSlotAvailableAsync(
                request.ServicePointId, safeStartTime, ct);

            if (!slotFree)
                throw new DomainException("The selected slot is already occupied.");

            var reservation = Reservation.Create(
                id: Guid.NewGuid(),
                servicePointId: request.ServicePointId,
                serviceId: request.ServiceId,
                userId: userId,
                startTime: safeStartTime,
                createdAt: _clock.UtcNow
            );

            await _reservations.AddAsync(reservation, ct);

            var ticketNo = GenerateTicketNo(request.ServicePointId);
            var entry = QueueEntry.Create(
                id: Guid.NewGuid(),
                servicePointId: request.ServicePointId,
                reservationId: reservation.Id,
                ticketNo: ticketNo,
                priority: 0,
                createdAt: _clock.UtcNow
            );

            await _queueEntries.AddAsync(entry, ct);

            await _realtime.QueueUpdated(request.ServicePointId, ct);

            return new ReservationDto
            {
                Id = reservation.Id,
                ServicePointId = reservation.ServicePointId,
                ServiceId = reservation.ServiceId,
                UserId = reservation.UserId,
                StartTime = reservation.StartTime,
                Status = reservation.Status.ToString(),
                TicketNo = entry.TicketNo
            };
        }

        private static string GenerateTicketNo(Guid servicePointId)
        {
            var s = servicePointId.ToString("N");
            var prefix = s.Substring(0, 3).ToUpperInvariant();
            var suffix = Guid.NewGuid().ToString("N")[..4].ToUpperInvariant();
            return $"{prefix}-{suffix}";
        }
    }
}
