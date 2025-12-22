window.qlineRealtime = {
    connect: function (servicePointId, dotNetRef) {
        const conn = new signalR.HubConnectionBuilder()
            .withUrl("/hubs/queue")
            .withAutomaticReconnect()
            .build();

        conn.on("queueUpdated", payload => {
            if (!payload || payload.servicePointId !== servicePointId) return;
            dotNetRef.invokeMethodAsync("OnQueueUpdated");
        });

        conn.start()
            .then(() => conn.invoke("JoinServicePoint", servicePointId))
            .catch(console.error);

        return {
            dispose: () => {
                conn.invoke("LeaveServicePoint", servicePointId)
                    .finally(() => conn.stop());
            }
        };
    },

    connectUserProfile: function (dotNetRef) {
        const conn = new signalR.HubConnectionBuilder()
            .withUrl("/hubs/queue")
            .withAutomaticReconnect()
            .build();

        conn.on("myReservationsUpdated", () => {
            console.log("[Realtime] Profile update received!");
            dotNetRef.invokeMethodAsync("OnQueueUpdated");
        });

        conn.start().catch(console.error);

        return {
            dispose: () => conn.stop()
        };
    }
};
