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
    public sealed class AppUserRepository : IAppUserRepository
    {
        private readonly AppDbContext _db;
        public AppUserRepository(AppDbContext db) => _db = db;

        public Task<AppUser?> GetByIdAsync(Guid id, CancellationToken ct) =>
            _db.AppUsers.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);

        public Task<AppUser?> GetByEmailAsync(Guid tenantId, string email, CancellationToken ct) =>
            _db.AppUsers.AsNoTracking()
                .FirstOrDefaultAsync(u => u.TenantId == tenantId && u.Email == email, ct);
        
        public async Task AddAsync(AppUser user, CancellationToken ct)
        {
            await _db.AppUsers.AddAsync(user, ct);
            await _db.SaveChangesAsync(ct);
        }
    }
}
