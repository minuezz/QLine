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

        public async Task<IReadOnlyList<ServicePoint>> GetAllAsync(CancellationToken ct) =>
            await _db.ServicePoints.AsNoTracking()
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

        public async Task AddStaffAssignmentAsync(StaffAssignment assignment, CancellationToken ct)
        {
            await _db.StaffAssignments.AddAsync(assignment, ct);
            await _db.SaveChangesAsync(ct);
        }

        public async Task RemoveStaffAssignmentAsync(Guid servicePointId, Guid userId, CancellationToken ct)
        {
            var assignment = await _db.StaffAssignments
                .FirstOrDefaultAsync(sa => sa.ServicePointId == servicePointId && sa.UserId == userId, ct);

            if (assignment is null) return;

            _db.StaffAssignments.Remove(assignment);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<IReadOnlyList<AppUser>> GetAssignedStaffAsync(Guid servicePointId, CancellationToken ct) =>
            await _db.StaffAssignments
                .AsNoTracking()
                .Where(sa => sa.ServicePointId == servicePointId)
                .Join(
                    _db.AppUsers.AsNoTracking(),
                    sa => sa.UserId,
                    u => u.Id,
                    (_, u) => u)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync(ct);

        public async Task<IReadOnlyList<ServicePoint>> GetAssignedServicePointsAsync(Guid userId, CancellationToken ct) =>
            await _db.StaffAssignments
                .AsNoTracking()
                .Where(sa => sa.UserId == userId)
                .Join(
                    _db.ServicePoints.AsNoTracking(),
                    sa => sa.ServicePointId,
                    sp => sp.Id,
                    (_, sp) => sp)
                .OrderBy(sp => sp.Name)
                .ToListAsync(ct);
    }
}
