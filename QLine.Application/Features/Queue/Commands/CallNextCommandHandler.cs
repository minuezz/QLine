using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using QLine.Application.Abstractions;
using QLine.Application.DTO;
using QLine.Domain.Abstractions;
using QLine.Domain.Entities;

namespace QLine.Application.Features.Queue.Commands
{
    public sealed class CallNextCommandHandler : IRequestHandler<CallNextCommand, QueueEntryDto?>
    {
        private readonly IQueueEntryRepository _repo;
        private readonly IRealtimeNotifier _realtime;
        private readonly IReservationRepository _reservations;

        public CallNextCommandHandler(IQueueEntryRepository repo, IRealtimeNotifier realtime, IReservationRepository reservations)
        {
            _repo = repo;
            _realtime = realtime;
            _reservations = reservations;
        }

        public async Task<QueueEntryDto?> Handle(CallNextCommand request, CancellationToken ct)
        {
            var current = await _repo.GetCurrentInServiceByServicePointAsync(request.ServicePointId, ct);
            if (current is not null)
                return ToDto(current);

            var next = await _repo.TryStartNextInServiceAsync(request.ServicePointId, ct);
            if (next is null) return null;

            var reservation = await _reservations.GetByIdAsync(next.ReservationId, ct);

            await _realtime.QueueUpdated(next.ServicePointId, ct);

            if (reservation != null)
            {
                reservation.StartService();
                await _reservations.UpdateAsync(reservation, ct);

                await _realtime.UserReservationsUpdated(reservation.UserId, ct);
            }
            return ToDto(next);
        }

        private static QueueEntryDto ToDto(QueueEntry entry) => new()
        {
            Id = entry.Id,
            TicketNo = entry.TicketNo,
            Status = entry.Status.ToString(),
            Priority = entry.Priority,
            CreatedAt = entry.CreatedAt
        };
    }
}
