using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLine.Application.Abstractions
{
    public interface IRealtimeNotifier
    {
        Task QueueUpdated(Guid tenantId, Guid servicePointId, CancellationToken ct = default);
    }
}
