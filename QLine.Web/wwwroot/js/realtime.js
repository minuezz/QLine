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
    }
};
