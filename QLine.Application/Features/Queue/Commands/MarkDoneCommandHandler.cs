using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using QLine.Application.Abstractions;
using QLine.Domain;
using QLine.Domain.Abstractions;

namespace QLine.Application.Features.Queue.Commands
{
    public sealed class MarkDoneCommandHandler : IRequestHandler<MarkDoneCommand>
    {
        private readonly IQueueEntryRepository _repo;
        private readonly IRealtimeNotifier _realtime;
        private readonly IReservationRepository _reservations;

        public MarkDoneCommandHandler(IQueueEntryRepository repo, IRealtimeNotifier realtime, IReservationRepository reservations)
        {
            _repo = repo;
            _realtime = realtime;
            _reservations = reservations;
        }

        public async Task Handle(MarkDoneCommand request, CancellationToken ct)
        {
            var entry = await _repo.GetByIdAsync(request.QueueEntryId, ct)
                ?? throw new DomainException("Queue entry not found.");

            entry.MarkDone();
            await _repo.UpdateAsync(entry, ct);

            await _realtime.QueueUpdated(entry.ServicePointId, ct);

            var reservation = await _reservations.GetByIdAsync(entry.ReservationId, ct);
            if (reservation != null)
            {
                reservation.Complete();
                await _reservations.UpdateAsync(reservation, ct);

                await _realtime.UserReservationsUpdated(reservation.UserId, ct);
            }
        }
    }
}
