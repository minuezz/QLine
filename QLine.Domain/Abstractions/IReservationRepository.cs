using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLine.Domain.Entities;

namespace QLine.Domain.Abstractions
{
    public interface IReservationRepository
    {
        /// <summary>
        /// Checking slot availability, taking into account active reservations.
        /// </summary>

        Task<bool> IsSlotAvailableAsync(Guid servicePointId, DateTimeOffset startTime, CancellationToken ct);

        Task AddAsync(Reservation reservation, CancellationToken ct);
        Task<Reservation?> GetByIdAsync(Guid id, CancellationToken ct);
    }
}
