using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLine.Application.DTO
{
    public sealed class ReservationDto
    {
        public Guid Id { get; init; }
        public Guid ServicePointId {  get; init; }
        public Guid ServiceId { get; init; }
        public Guid UserId {  get; init; }
        public DateTimeOffset StartTime { get; init; }
        public string Status { get; init; } = default!;
        public string? TicketNo { get; init; }
    }
}
