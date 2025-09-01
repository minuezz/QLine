using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QLine.Domain.Abstractions;
using QLine.Domain.Entities;
using QLine.Domain.Enums;

namespace QLine.Infrastructure.Persistence.Repositories
{
    public sealed class QueueEntryRepository : IQueueEntryRepository
    {
        private readonly AppDbContext _db;
        public QueueEntryRepository(AppDbContext db) => _db = db;

        public async Task AddAsync(QueueEntry entry, CancellationToken ct)
        {
            await _db.QueueEntries.AddAsync(entry, ct);
            await _db.SaveChangesAsync(ct);
        }

        public Task<QueueEntry?> GetByIdAsync(Guid id, CancellationToken ct) =>
            _db.QueueEntries.AsNoTracking().FirstOrDefaultAsync(q => q.Id == id, ct);

        public async Task<IReadOnlyList<QueueEntry>> GetWaitingByServicePointAsync(Guid servicePointId, CancellationToken ct) =>
            await _db.QueueEntries.AsNoTracking()
                .Where(q => q.ServicePointId == servicePointId && q.Status == QueueStatus.Waiting)
                .OrderByDescending(q => q.Priority)
                .ThenBy(q => q.CreatedAt)
                .ToListAsync(ct);
    }
}
