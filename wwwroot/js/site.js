// SignalR - Notificações em tempo real
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .withAutomaticReconnect()
    .build();

let notifCount = 0;

connection.on("ReceiveNotification", (data) => {
    notifCount++;
    document.getElementById("notif-count").textContent = notifCount;

    const list = document.getElementById("notif-list");
    const item = document.createElement("li");
    item.innerHTML = `
        <strong>${data.title}</strong><br/>
        <span>${data.message}</span><br/>
        <small style="color:#aaa">${data.time}</small>
    `;
    list.prepend(item);

    showToast(data.title, data.message, data.type);
});

connection.start().catch(err => console.error("SignalR error:", err));

// Toggle painel de notificações
document.getElementById("notification-bell").addEventListener("click", () => {
    const panel = document.getElementById("notification-panel");
    panel.classList.toggle("hidden");
    notifCount = 0;
    document.getElementById("notif-count").textContent = "0";
});

// Toast de notificação
function showToast(title, message, type) {
    const toast = document.createElement("div");
    toast.style.cssText = `
        position: fixed;
        bottom: 20px;
        right: 20px;
        background: #1a1d27;
        border: 1px solid ${type === 'reminder' ? '#e74c3c' : type === 'warning' ? '#f39c12' : '#7c6af7'};
        border-radius: 12px;
        padding: 1rem 1.5rem;
        z-index: 9999;
        max-width: 320px;
        animation: slideIn 0.3s ease;
    `;
    toast.innerHTML = `
        <strong style="color:#fff">${title}</strong>
        <p style="color:#aaa;margin-top:4px;font-size:0.9rem">${message}</p>
    `;
    document.body.appendChild(toast);
    setTimeout(() => toast.remove(), 5000);
}

// Animação do toast
const style = document.createElement("style");
style.textContent = `
    @keyframes slideIn {
        from { transform: translateX(100px); opacity: 0; }
        to { transform: translateX(0); opacity: 1; }
    }
`;
document.head.appendChild(style);''