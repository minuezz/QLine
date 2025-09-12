using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using QLine.Domain;
using QLine.Domain.Abstractions;

namespace QLine.Application.Features.Queue.Commands
{
    public sealed class MarkNoShowCommandHandler : IRequestHandler<MarkNoShowCommand>
    {
        private readonly IQueueEntryRepository _repo;

        public MarkNoShowCommandHandler(IQueueEntryRepository repo) => _repo = repo;

        public async Task Handle(MarkNoShowCommand request, CancellationToken ct)
        {
            var entry = await _repo.GetByIdAsync(request.QueueEntryId, ct)
                ?? throw new DomainException("Queue entry not found.");
            entry.MarkNoShow();
            await _repo.UpdateAsync(entry, ct);
        }
    }
}
