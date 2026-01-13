using Microsoft.AspNetCore.SignalR;

namespace QLine.Web.Hubs
{
    public sealed class QueueHub : Hub
    {
        public static string GroupName(Guid servicePointId)
            => $"sp:{servicePointId}";

        public Task JoinServicePoint(Guid servicePointId)
            => Groups.AddToGroupAsync(Context.ConnectionId, GroupName(servicePointId));

        public Task LeaveServicePoint(Guid servicePointId)
            => Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(servicePointId));
    }
}
