using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QLine.Domain.Abstractions;
using QLine.Domain.Entities;

namespace QLine.Infrastructure.Persistence.Repositories
{
    public sealed class ServiceRepository : IServiceRepository
    {
        private readonly AppDbContext _db;
        public ServiceRepository(AppDbContext db) => _db = db;

        public async Task<IReadOnlyList<Service>> GetByServicePointAsync(Guid servicePointId, CancellationToken ct) =>
            await _db.Services.AsNoTracking()
                .Where(s => s.ServicePointId == servicePointId)
                .OrderBy(s => s.Name)
                .ToListAsync(ct);

        public Task<Service?> GetByIdAsync(Guid id, CancellationToken ct) =>
            _db.Services.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id, ct);

        public async Task AddAsync(Service svc, CancellationToken ct)
        {
            await _db.Services.AddAsync(svc, ct);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Service svc, CancellationToken ct)
        {
            _db.Services.Update(svc);
            await _db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct)
        {
            var entity = await _db.Services.FindAsync(new object?[] { id }, ct);
            if (entity is null) return;
            _db.Services.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}
