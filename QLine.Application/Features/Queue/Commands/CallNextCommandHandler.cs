using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using QLine.Application.DTO;
using QLine.Domain.Abstractions;
namespace QLine.Application.Features.Queue.Commands
{
    public sealed class CallNextCommandHandler : IRequestHandler<CallNextCommand, QueueEntryDto?>
    {
        private readonly IQueueEntryRepository _repo;

        public CallNextCommandHandler(IQueueEntryRepository repo) => _repo = repo;

        public async Task<QueueEntryDto?> Handle(CallNextCommand request, CancellationToken ct)
        {
            var current = await _repo.GetCurrentInServiceByServicePointAsync(request.ServicePointId, ct);
            if (current is not null) return new QueueEntryDto
            {
                Id = current.Id,
                TicketNo = current.TicketNo,
                Status = current.Status.ToString(),
                Priority = current.Priority,
                CreatedAt = current.CreatedAt
            };

            var next = await _repo.GetFirstWaitingByServicePointAsync(request.ServicePointId, ct);
            if (next is null) return null;

            next.MarkInService();
            await _repo.UpdateAsync(next, ct);

            return new QueueEntryDto
            {
                Id = next.Id,
                TicketNo = next.TicketNo,
                Status = next.Status.ToString(),
                Priority = next.Priority,
                CreatedAt = next.CreatedAt
            };
        }
    }
}
