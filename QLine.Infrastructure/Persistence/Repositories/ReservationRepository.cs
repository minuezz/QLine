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
    public sealed class ReservationRepository : IReservationRepository
    {
        private readonly AppDbContext _db;
        public ReservationRepository(AppDbContext db) => _db = db;

        public async Task<bool> IsSlotAvailableAsync(Guid servicePointId, DateTimeOffset startTime, CancellationToken ct)
        {
            var isTaken = await _db.Reservations.AsNoTracking()
                .AnyAsync(r => r.ServicePointId == servicePointId
                            && r.StartTime == startTime
                            && r.Status == ReservationStatus.Active, ct);

            return !isTaken;
        }

        public async Task AddAsync(Reservation reservation, CancellationToken ct)
        {
            await _db.Reservations.AddAsync(reservation, ct);
            await _db.SaveChangesAsync(ct);
        }

        public Task<Reservation?> GetByIdAsync(Guid id, CancellationToken ct) =>
            _db.Reservations.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id, ct);

        public async Task<IReadOnlyList<Reservation>> GetByUserAsync(Guid userId, CancellationToken ct) =>
            await _db.Reservations.AsNoTracking()
                .Where(r => r.UserId == userId)
                .OrderBy(r => r.StartTime)
                .ToListAsync(ct);
    }
}
