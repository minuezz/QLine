using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
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
            _db.QueueEntries.FirstOrDefaultAsync(q => q.Id == id, ct);

        public Task<QueueEntry?> GetByReservationAsync(Guid reservationId, CancellationToken ct) =>
            _db.QueueEntries.AsNoTracking().FirstOrDefaultAsync(q => q.ReservationId == reservationId, ct);

        public Task<QueueEntry?> GetCurrentInServiceByServicePointAsync(Guid servicePointId, CancellationToken ct) =>
            _db.QueueEntries.FirstOrDefaultAsync(q => q.ServicePointId == servicePointId && q.Status == QueueStatus.InService, ct);

        public Task<QueueEntry?> GetFirstWaitingByServicePointAsync(Guid servicePointId, CancellationToken ct) =>
            _db.QueueEntries
                .Where(q => q.ServicePointId == servicePointId && q.Status == QueueStatus.Waiting)
                .OrderByDescending(q => q.Priority).ThenBy(q => q.CreatedAt)
                .FirstOrDefaultAsync(ct);

        public async Task<IReadOnlyList<QueueEntry>> GetWaitingByServicePointAsync(Guid servicePointId, CancellationToken ct) =>
            await _db.QueueEntries.AsNoTracking()
                .Where(q => q.ServicePointId == servicePointId && q.Status == QueueStatus.Waiting)
                .OrderByDescending(q => q.Priority)
                .ThenBy(q => q.CreatedAt)
                .ToListAsync(ct);

        public async Task<QueueEntry?> TryStartNextInServiceAsync(Guid servicePointId, CancellationToken ct)
        {
            await using var tx = await _db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);

            var next = await _db.QueueEntries
                .FromSqlInterpolated($@"
                    SELECT * FROM ""QueueEntries""
                    WHERE ""ServicePointId"" = {servicePointId}
                      AND ""Status"" = {(int)QueueStatus.Waiting}
                    ORDER BY ""Priority"" DESC, ""CreatedAt""
                    FOR UPDATE SKIP LOCKED
                    LIMIT 1")
                .FirstOrDefaultAsync(ct);

            if (next is null)
            {
                await tx.CommitAsync(ct);
                return null;
            }

            next.MarkInService();
            _db.QueueEntries.Update(next);
            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            return next;
        }

        public async Task UpdateAsync(QueueEntry entry, CancellationToken ct)
        {
            _db.QueueEntries.Update(entry);
            await _db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(QueueEntry entry, CancellationToken ct)
        {
            _db.QueueEntries.Remove(entry);
            await _db.SaveChangesAsync(ct);
        }
    }
}
