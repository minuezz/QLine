using Microsoft.EntityFrameworkCore;
using QLine.Domain.Abstractions;
using QLine.Domain.Entities;
using QLine.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLine.Infrastructure.Persistence.Repositories
{
    public sealed class AppUserRepository : IAppUserRepository
    {
        private readonly AppDbContext _db;
        public AppUserRepository(AppDbContext db) => _db = db;

        public Task<AppUser?> GetByIdAsync(Guid id, CancellationToken ct) =>
            _db.AppUsers.FirstOrDefaultAsync(u => u.Id == id, ct);

        public Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct) =>
            _db.AppUsers
                .FirstOrDefaultAsync(u => u.Email == email, ct);

        public Task<IReadOnlyList<AppUser>> GetAllAsync(CancellationToken ct) =>
            _db.AppUsers
                .AsNoTracking()
                .ToListAsync(ct);

        public async Task AddAsync(AppUser user, CancellationToken ct)
        {
            await _db.AppUsers.AddAsync(user, ct);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(AppUser user, CancellationToken ct)
        {
            _db.AppUsers.Update(user);
            await _db.SaveChangesAsync(ct);
        }

        public async Task DeleteWithRelatedDataAsync(AppUser user, CancellationToken ct)
        {
            var reservations = await _db.Reservations
                .Where(r => r.UserId == user.Id)
                .ToListAsync(ct);

            foreach (var reservation in reservations.Where(r => r.Status == ReservationStatus.Active))
            {
                reservation.Cancel();
            }

            var reservationIds = reservations.Select(r => r.Id).ToList();
            if (reservationIds.Any())
            {
                var queueEntries = await _db.QueueEntries
                    .Where(q => reservationIds.Contains(q.ReservationId))
                    .ToListAsync(ct);
                _db.QueueEntries.RemoveRange(queueEntries);
            }

            _db.Reservations.RemoveRange(reservations);
            _db.AppUsers.Remove(user);

            await _db.SaveChangesAsync(ct);
        }
    }
}
