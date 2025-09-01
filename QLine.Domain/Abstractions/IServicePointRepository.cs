using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLine.Domain.Entities;

namespace QLine.Domain.Abstractions
{
    public interface IServicePointRepository
    {
        Task<IReadOnlyList<ServicePoint>> GetByTenantAsync(Guid tenantId, CancellationToken ct);
        Task<ServicePoint?> GetByIdAsync(Guid id, CancellationToken ct);
    }
}
