using Microsoft.AspNetCore.SignalR;
using QLine.Application.Abstractions;
using QLine.Web.Hubs;

namespace QLine.Web.Services
{
    public sealed class SignalRRealtimeNotifier : IRealtimeNotifier
    {
        private readonly IHubContext<QueueHub> _hub;
        public SignalRRealtimeNotifier(IHubContext<QueueHub> hub) => _hub = hub;

        public Task QueueUpdated(Guid servicePointId, CancellationToken ct = default)
            => _hub.Clients.Group(QueueHub.GroupName(servicePointId))
                .SendAsync("queueUpdated", new { servicePointId }, ct);

        public Task UserReservationsUpdated(Guid userId, CancellationToken ct = default)
            => _hub.Clients.User(userId.ToString())
                .SendAsync("myReservationsUpdated", ct);
    }
}
