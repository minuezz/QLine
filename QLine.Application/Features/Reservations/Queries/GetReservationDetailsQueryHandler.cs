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

            string? ticketNo = null;

            var list = await _queueEntries.GetWaitingByServicePointAsync(res.ServicePointId, ct);
            var linked = list.FirstOrDefault(q => q.ReservationId == res.Id);
            ticketNo = linked?.TicketNo;

            return new ReservationDto()
            {
                Id = res.Id,
                TenantId = res.TenantId,
                ServicePointId = res.ServicePointId,
                ServiceId = res.ServiceId,
                UserId = res.UserId,
                StartTime = res.StartTime,
                Status = res.Status.ToString(),
                TicketNo = ticketNo
            };
        }
    }
}
