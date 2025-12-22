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

    connectToServicePoints: function (servicePointIds, dotNetRef) {
        const conn = new signalR.HubConnectionBuilder()
            .withUrl("/hubs/queue")
            .withAutomaticReconnect()
            .build();

        let joined = new Set();

        const normalize = (ids) => new Set((ids || []).map(id => id?.toString?.() ?? id));

        const syncGroups = (ids) => {
            const desired = normalize(ids);

            for (const id of joined) {
                if (!desired.has(id)) {
                    conn.invoke("LeaveServicePoint", id).catch(console.error);
                }
            }

            for (const id of desired) {
                if (!joined.has(id)) {
                    conn.invoke("JoinServicePoint", id).catch(console.error);
                }
            }

            joined = desired;
        };

        conn.on("queueUpdated", payload => {
            if (!payload || !joined.has(payload.servicePointId)) return;
            dotNetRef.invokeMethodAsync("OnQueueUpdated");
        });

        conn.start()
            .then(() => syncGroups(servicePointIds))
            .catch(console.error);

        return {
            updateServicePoints: (ids) => syncGroups(ids),
            dispose: () => conn.stop()
        };
    }
};
