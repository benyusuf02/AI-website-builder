"use strict";

// 1. Identify User (Guest or Logged In - handled by Hub logic, we just provide the ID)
function getDeviceId() {
    let id = localStorage.getItem('ydev_chat_device_id');
    if (!id) {
        id = 'guest_' + Math.random().toString(36).substr(2, 9) + Date.now();
        localStorage.setItem('ydev_chat_device_id', id);
    }
    return id;
}
const deviceId = getDeviceId();

// 2. Connector
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .withAutomaticReconnect()
    .build();

// 3. UI Elements
const chatWidget = document.getElementById('chat-widget');
const chatMessages = document.getElementById('chat-messages');
const chatInput = document.getElementById('chat-input');
const sendButton = document.getElementById('chat-send-btn');
const openButton = document.getElementById('chat-open-btn');
const closeButton = document.getElementById('chat-close-btn');

// 4. Connection Logic
// 4. Connection Logic
let activeUserId = null;

async function startChat() {
    try {
        if (connection.state === signalR.HubConnectionState.Disconnected) {
            await connection.start();
            console.log("Connected to ChatHub");
        }

        // HANDSHAKE: Join the session room immediately & Get Real ID
        activeUserId = await connection.invoke("StartSession", deviceId);
        console.log("Active User ID:", activeUserId);

        sendButton.disabled = false;

    } catch (err) {
        console.error("Connection failed: ", err);
        setTimeout(startChat, 5000); // Retry
    }
}

// 5. Event Listeners (Hub)
connection.on("ReceiveMessage", (sender, message) => {
    // Check against Server-Confirmed ID first, then fallback to deviceId
    const isMe = (activeUserId && sender === activeUserId) || sender === deviceId || sender === "Me";
    const type = isMe ? 'user' : 'support';
    appendMessage(message, type);

    // Notify if closed
    if (chatWidget.classList.contains('d-none')) {
        openButton.classList.add('animate-bounce');
    }
});

// 6. Event Listeners (UI)
openButton.addEventListener('click', () => {
    chatWidget.classList.remove('d-none');
    chatWidget.classList.add('d-flex');
    openButton.classList.add('d-none');
    // Re-verify session on open (optional but good for robustness)
    if (connection.state === signalR.HubConnectionState.Connected) {
        connection.invoke("StartSession", deviceId).catch(console.error);
    }
});

closeButton.addEventListener('click', () => {
    chatWidget.classList.add('d-none');
    chatWidget.classList.remove('d-flex');
    openButton.classList.remove('d-none');
    openButton.classList.remove('animate-bounce');
});

async function sendMessage() {
    const text = chatInput.value.trim();
    if (!text) return;

    try {
        await connection.invoke("SendMessage", text, deviceId);
        chatInput.value = '';
        // Note: We don't appendMessage here manually anymore, strictly waiting for server echo via ReceiveMessage
        // This ensures what we see is what is in the server. 
        // If latency is high, we can optimistic append, but for reliability "Server Truth" is better.
    } catch (err) {
        console.error("Send failed: ", err);
        appendMessage("⚠️ Mesaj gönderilemedi.", 'system');
    }
}

sendButton.addEventListener('click', (e) => {
    e.preventDefault();
    sendMessage();
});

chatInput.addEventListener('keypress', (e) => {
    if (e.key === 'Enter') {
        e.preventDefault();
        sendMessage();
    }
});

// 7. Helper
function appendMessage(text, type) {
    const msgDiv = document.createElement('div');
    msgDiv.classList.add('message', type);
    msgDiv.textContent = text;
    chatMessages.appendChild(msgDiv);
    chatMessages.scrollTop = chatMessages.scrollHeight;
}

// Init
startChat();
