using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QLine.Application.Abstractions;
using QLine.Application.DTO;
using QLine.Domain;
using QLine.Domain.Abstractions;
using QLine.Domain.Entities;
using QLine.Domain.Enums;

namespace QLine.Application.Features.Reservations.Commands
{
    public class CheckInByQrCommandHandler : IRequestHandler<CheckInByQrCommand, QueueSnapshotDto>
    {
        private readonly IReservationRepository _reservations;
        private readonly IQueueEntryRepository _queueEntries;
        private readonly IDateTimeProvider _clock;
        private readonly IRealtimeNotifier _realtime;

        public CheckInByQrCommandHandler(
            IReservationRepository reservations,
            IQueueEntryRepository queueEntries,
            IDateTimeProvider clock,
            IRealtimeNotifier realtime)
        {
            _reservations = reservations;
            _queueEntries = queueEntries;
            _clock = clock;
            _realtime = realtime;
        }

        public async Task<QueueSnapshotDto> Handle(CheckInByQrCommand request, CancellationToken ct)
        {
            var reservation = await _reservations.GetByIdAsync(request.ReservationId, ct);
            var now = _clock.UtcNow;

            if (reservation is null || reservation.Status != ReservationStatus.Active)
            {
                throw new DomainException("Reservation is invalid or not active.");
            }

            var timeDifference = reservation.StartTime - now;
            if (timeDifference.TotalHours > 20 || timeDifference.TotalHours < -2)
            {
                throw new DomainException("It is too early or too late to check in.");
            }

            var existingEntry = await _queueEntries.GetByReservationAsync(reservation.Id, ct);
            if (existingEntry is not null)
            {
                throw new DomainException("Reservation is already checked in.");
            }

            var ticketNo = GenerateTicketNumber(reservation);

            var entry = QueueEntry.Create(
                Guid.NewGuid(),
                reservation.ServicePointId,
                reservation.Id,
                ticketNo,
                priority: 0,
                createdAt: _clock.UtcNow);

            await _queueEntries.AddAsync(entry, ct);

            reservation.CheckIn();
            await _reservations.UpdateAsync(reservation, ct);

            await _realtime.QueueUpdated(entry.ServicePointId, ct);

            await _realtime.UserReservationsUpdated(reservation.UserId, ct);

            var current = await _queueEntries.GetCurrentInServiceByServicePointAsync(entry.ServicePointId, ct);
            var waiting = await _queueEntries.GetWaitingByServicePointAsync(entry.ServicePointId, ct);

            return new QueueSnapshotDto
            {
                Current = current is null ? null : new QueueEntryDto
                {
                    Id = current.Id,
                    TicketNo = current.TicketNo,
                    Status = current.Status.ToString(),
                    Priority = current.Priority,
                    CreatedAt = current.CreatedAt
                },
                Waiting = waiting.Select(w => new QueueEntryDto
                {
                    Id = w.Id,
                    TicketNo = w.TicketNo,
                    Status = w.Status.ToString(),
                    Priority = w.Priority,
                    CreatedAt = w.CreatedAt
                }).ToList()
            };
        }

        private string GenerateTicketNumber(Reservation reservation)
        {
            var prefix = reservation.ServicePointId
                .ToString("N")
                .Substring(0, 4)
                .ToUpperInvariant();

            var randomNumber = RandomNumberGenerator.GetInt32(100, 10000);
            return $"{prefix}-{randomNumber:0000}";
        }
    }
}
