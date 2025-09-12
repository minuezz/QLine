﻿using System;
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
            _db.ServicePoints.AsNoTracking().FirstOrDefaultAsync(sp => sp.Id == id, ct);
    }
}
