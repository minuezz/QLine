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
            var result = new List<ReservationDto>();

            foreach (var r in reservations)
            {
                var entry = await _queueEntries.GetByReservationAsync(r.Id, ct);

                result.Add(new ReservationDto
                {
                    Id = r.Id,
                    ServicePointId = r.ServicePointId,
                    ServiceId = r.ServiceId,
                    UserId = r.UserId,
                    StartTime = r.StartTime,
                    Status = r.Status.ToString(),
                    TicketNo = entry?.TicketNo
                });
            }

            return result;
        }
    }
}
