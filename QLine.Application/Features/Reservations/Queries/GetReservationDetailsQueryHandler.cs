using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using QLine.Application.DTO;
using QLine.Domain;
using QLine.Domain.Abstractions;

namespace QLine.Application.Features.Reservations.Queries
{
    public sealed class GetReservationDetailsQueryHandler
        : IRequestHandler<GetReservationDetailsQuery, ReservationDto>
    {
        private readonly IReservationRepository _reservations;
        private readonly IQueueEntryRepository _queueEntries;

        public GetReservationDetailsQueryHandler(
            IReservationRepository reservations,
            IQueueEntryRepository queueEntries)
        {
            _reservations = reservations;
            _queueEntries = queueEntries;
        }

        public async Task<ReservationDto> Handle(GetReservationDetailsQuery request, CancellationToken ct)
        {
            var res = await _reservations.GetByIdAsync(request.ReservationId, ct)
                ?? throw new DomainException("Reservation not found.");

            var linked = await _queueEntries.GetByReservationAsync(res.Id, ct);

            return new ReservationDto()
            {
                Id = res.Id,
                ServicePointId = res.ServicePointId,
                ServiceId = res.ServiceId,
                UserId = res.UserId,
                StartTime = res.StartTime,
                Status = res.Status.ToString(),
                TicketNo = linked?.TicketNo
            };
        }
    }
}
