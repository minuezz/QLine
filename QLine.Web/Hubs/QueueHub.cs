using Microsoft.AspNetCore.SignalR;

namespace QLine.Web.Hubs
{
    public sealed class QueueHub : Hub
    {
        public static string GroupName(Guid tenantId, Guid servicePointId)
            => $"tenant: {tenantId}:sp:{servicePointId}";

        public Task JoinServicePoint(Guid tenantId, Guid servicePointId)
            => Groups.AddToGroupAsync(Context.ConnectionId, GroupName(tenantId, servicePointId));

        public Task LeaveServicePoint(Guid tenantId, Guid servicePointId)
            => Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(tenantId, servicePointId));
    }
}
