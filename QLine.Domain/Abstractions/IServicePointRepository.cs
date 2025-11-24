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
        Task<IReadOnlyList<ServicePoint>> GetAllAsync(CancellationToken ct);
        Task<ServicePoint?> GetByIdAsync(Guid id, CancellationToken ct);
        Task AddAsync(ServicePoint sp, CancellationToken ct);
        Task UpdateAsync(ServicePoint sp, CancellationToken ct);
        Task DeleteAsync(Guid id, CancellationToken ct);
    }
}
