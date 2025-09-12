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
    public sealed class TenantRepository : ITenantRepository
    {
        private readonly AppDbContext _db;
        public TenantRepository(AppDbContext db) => _db = db;

        public Task<Tenant?> GetBySlugAsync(string slug, CancellationToken ct) =>
            _db.Tenants.AsNoTracking().FirstOrDefaultAsync(t => t.Slug == slug, ct);

        public Task<Tenant?> GetByIdAsync(Guid id, CancellationToken ct) =>
            _db.Tenants.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, ct);

        public Task<bool> ExistsByIdAsync(Guid id, CancellationToken ct) =>
            _db.Tenants.AsNoTracking().AnyAsync(t => t.Id == id, ct);
    }
}
