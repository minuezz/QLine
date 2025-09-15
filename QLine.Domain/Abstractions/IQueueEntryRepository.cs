using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLine.Domain.Entities;

namespace QLine.Domain.Abstractions
{
    public interface IQueueEntryRepository
    {
        Task AddAsync(QueueEntry entry, CancellationToken ct);
        Task<QueueEntry?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<IReadOnlyList<QueueEntry>> GetWaitingByServicePointAsync(Guid servicePointId, CancellationToken ct);
        Task<QueueEntry?> GetCurrentInServiceByServicePointAsync(Guid servicePointId, CancellationToken ct);
        Task<QueueEntry?> GetFirstWaitingByServicePointAsync(Guid servicePointId, CancellationToken ct);
        Task<QueueEntry?> TryStartNextInServiceAsync(Guid servicePointId, CancellationToken ct);
        Task UpdateAsync(QueueEntry entry, CancellationToken ct);
    }
}
