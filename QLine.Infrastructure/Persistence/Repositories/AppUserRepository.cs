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
            _db.AppUsers.FirstOrDefaultAsync(u => u.Id == id, ct);

        public Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct) =>
            _db.AppUsers
                .FirstOrDefaultAsync(u => u.Email == email, ct);

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
    }
}
