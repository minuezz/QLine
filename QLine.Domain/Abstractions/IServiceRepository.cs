using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLine.Domain.Entities;

namespace QLine.Domain.Abstractions
{
    public interface IServiceRepository
    {
        Task<IReadOnlyList<Service>> GetByServicePointAsync(Guid servicePointId, CancellationToken ct);
        Task<Service?> GetByIdAsync(Guid id,  CancellationToken ct);
    }
}
