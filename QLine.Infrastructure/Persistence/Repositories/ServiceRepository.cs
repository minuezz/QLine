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
    }
}
