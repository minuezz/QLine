using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLine.Domain.Entities;

namespace QLine.Domain.Abstractions
{
    public interface IAppUserRepository
    {
        Task<AppUser?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct);
        Task AddAsync (AppUser user, CancellationToken ct);
    }
}
