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
    public sealed class ServicePointRepository : IServicePointRepository
    {
        private readonly AppDbContext _db;
        public ServicePointRepository(AppDbContext db) => _db = db;

        public async Task<IReadOnlyList<ServicePoint>> GetByTenantAsync(Guid tenantId, CancellationToken ct) =>
            await _db.ServicePoints.AsNoTracking()
                .Where(sp => sp.TenantId == tenantId)
                .OrderBy(sp => sp.Name)
                .ToListAsync(ct);
        
        public Task<ServicePoint?> GetByIdAsync(Guid id, CancellationToken ct) =>
            _db.ServicePoints.FirstOrDefaultAsync(sp => sp.Id == id, ct);

        public async Task AddAsync(ServicePoint sp, CancellationToken ct)
        {
            await _db.ServicePoints.AddAsync(sp, ct);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(ServicePoint sp, CancellationToken ct)
        {
            _db.ServicePoints.Update(sp);
            await _db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct)
        {
            var entity = await _db.ServicePoints.FindAsync(new object?[] { id }, ct);
            if (entity is null) return;
            _db.ServicePoints.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}
