using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using QLine.Application.Abstractions;
using QLine.Domain.Abstractions;
using QLine.Domain;

namespace QLine.Application.Features.Queue.Commands
{
    public sealed class SkipQueueEntryCommandHandler : IRequestHandler<SkipQueueEntryCommand>
    {
        private readonly IQueueEntryRepository _repo;
        private readonly IRealtimeNotifier _realtime;

        public SkipQueueEntryCommandHandler(IQueueEntryRepository repo, IRealtimeNotifier realtime)
        {
            _repo = repo;
            _realtime = realtime;
        }

        public async Task Handle(SkipQueueEntryCommand request, CancellationToken ct)
        {
            var e = await _repo.GetByIdAsync(request.QueueEntryId, ct)
                ?? throw new DomainException("Queue entry not found.");
            e.Skip();
            await _repo.UpdateAsync(e, ct);
            await _realtime.QueueUpdated(e.ServicePointId, ct);
        }
    }
}
