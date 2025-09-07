window.qlineTime = {
    getTimeZone: () => {
        Intl.DateTimeFormat().resolvedOptions().timeZone || null
    }
};
