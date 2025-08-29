using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLine.Domain.Entities;

namespace QLine.Domain.Abstractions
{
    public interface ITenantRepository
    {
        Task<Tenant?> GetBySlugAsync(string slug, CancellationToken ct);
        Task<Tenant?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<bool> ExistsByIdAsync(Guid id, CancellationToken ct);
    }
}
