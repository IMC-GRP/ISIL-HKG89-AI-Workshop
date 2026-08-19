window.assistantChat = {
    scrollToBottom: function (elementId) {
        const container = document.getElementById(elementId);
        if (!container) {
            return;
        }

        container.scrollTop = container.scrollHeight;
    },
    focusInput: function (elementId) {
        const input = document.getElementById(elementId);
        if (!input) {
            return;
        }

        input.focus({ preventScroll: true });
    }
};