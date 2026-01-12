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
            dotNetRef.invokeMethodAsync("OnQueueUpdated");
        });

        conn.start().catch(console.error);

        return {
            dispose: () => conn.stop()
        };
    }
};

window.qlineSound = {
    playTone: function () {
        try {
            const AudioContext = window.AudioContext || window.webkitAudioContext;
            const ctx = new AudioContext();
            const oscillator = ctx.createOscillator();
            const gainNode = ctx.createGain();

            oscillator.type = "sine";
            oscillator.frequency.setValueAtTime(880, ctx.currentTime);

            gainNode.gain.setValueAtTime(0.001, ctx.currentTime);
            gainNode.gain.exponentialRampToValueAtTime(0.25, ctx.currentTime + 0.01);
            gainNode.gain.exponentialRampToValueAtTime(0.0001, ctx.currentTime + 0.4);

            oscillator.connect(gainNode);
            gainNode.connect(ctx.destination);

            oscillator.start();
            oscillator.stop(ctx.currentTime + 0.4);
        } catch (err) {
            console.error("Failed to play notification sound", err);
        }
    }
};
