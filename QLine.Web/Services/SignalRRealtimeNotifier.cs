using Microsoft.AspNetCore.SignalR;
using QLine.Application.Abstractions;
using QLine.Web.Hubs;

namespace QLine.Web.Services
{
    public sealed class SignalRRealtimeNotifier : IRealtimeNotifier
    {
        private readonly IHubContext<QueueHub> _hub;
        public SignalRRealtimeNotifier(IHubContext<QueueHub> hub) => _hub = hub;

        public Task QueueUpdated(Guid tenantId, Guid servicePointId, CancellationToken ct = default)
            => _hub.Clients.Group(QueueHub.GroupName(tenantId, servicePointId))
                .SendAsync("queueUpdated", new { tenantId, servicePointId }, ct);
    }
}
