using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using QLine.Application.DTO;
using QLine.Domain.Abstractions;

namespace QLine.Application.Features.Reservations.Queries
{
    public sealed class GetMyReservationsQueryHandler
        : IRequestHandler<GetMyReservationsQuery, IReadOnlyList<ReservationDto>>
    {
        private readonly IReservationRepository _reservations;
        private readonly IQueueEntryRepository _queueEntries;

        public GetMyReservationsQueryHandler(
            IReservationRepository reservations,
            IQueueEntryRepository queueEntries)
        {
            _reservations = reservations;
            _queueEntries = queueEntries;
        }

        public async Task<IReadOnlyList<ReservationDto>> Handle(GetMyReservationsQuery request, CancellationToken ct)
        {
            var reservations = await _reservations.GetByUserAsync(request.UserId, ct);

            var entries = await Task.WhenAll(reservations.Select(async r => new
            {
                Reservation = r,
                Entry = await _queueEntries.GetByReservationAsync(r.Id, ct)
            }));

            return entries
                .Select(x => new ReservationDto
                {
                    Id = x.Reservation.Id,
                    ServicePointId = x.Reservation.ServicePointId,
                    ServiceId = x.Reservation.ServiceId,
                    UserId = x.Reservation.UserId,
                    StartTime = x.Reservation.StartTime,
                    Status = x.Reservation.Status.ToString(),
                    TicketNo = x.Entry?.TicketNo
                })
                .ToList();
        }
    }
}
