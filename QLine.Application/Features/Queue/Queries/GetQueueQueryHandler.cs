using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using QLine.Application.DTO;
using QLine.Domain.Abstractions;

namespace QLine.Application.Features.Queue.Queries
{
    public sealed class GetQueueQueryHandler : IRequestHandler<GetQueueQuery, QueueSnapshotDto>
    {
        private readonly IQueueEntryRepository _repo;

        public GetQueueQueryHandler(IQueueEntryRepository repo) => _repo = repo;

        public async Task<QueueSnapshotDto> Handle(GetQueueQuery request, CancellationToken ct)
        {
            var current = await _repo.GetCurrentInServiceByServicePointAsync(request.ServicePointId, ct);
            var waiting = await _repo.GetWaitingByServicePointAsync(request.ServicePointId, ct);

            return new QueueSnapshotDto
            {
                Current = current is null ? null : new QueueEntryDto
                {
                    Id = current.Id,
                    TicketNo = current.TicketNo,
                    Status = current.Status.ToString(),
                    Priority = current.Priority,
                    CreatedAt = current.CreatedAt
                },
                Waiting = waiting.Select(w => new QueueEntryDto
                {
                    Id = w.Id,
                    TicketNo = w.TicketNo,
                    Status = w.Status.ToString(),
                    Priority = w.Priority,
                    CreatedAt = w.CreatedAt
                }).ToList()
            };
        }
    }
}
