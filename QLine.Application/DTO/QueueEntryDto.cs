using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLine.Application.DTO
{
    public sealed class QueueEntryDto
    {
        public Guid Id { get; init; }
        public string TicketNo { get; init; } = default!;
        public string Status { get; init; } = default!;
        public int Priority { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
    }
}
