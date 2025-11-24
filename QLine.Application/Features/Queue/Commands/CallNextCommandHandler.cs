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

        public CallNextCommandHandler(IQueueEntryRepository repo, IRealtimeNotifier realtime)
        {
            _repo = repo;
            _realtime = realtime;
        }

        public async Task<QueueEntryDto?> Handle(CallNextCommand request, CancellationToken ct)
        {
            var current = await _repo.GetCurrentInServiceByServicePointAsync(request.ServicePointId, ct);
            if (current is not null)
                return ToDto(current);

            var next = await _repo.TryStartNextInServiceAsync(request.ServicePointId, ct);
            if (next is null) return null;

            await _realtime.QueueUpdated(next.ServicePointId, ct);

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
