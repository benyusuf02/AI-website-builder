using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using YDeveloper.Data;
using YDeveloper.Models;

namespace YDeveloper.Hubs
{
    public class ChatHub : Hub
    {
        private readonly YDeveloperContext _context;

        public ChatHub(YDeveloperContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Called by the Client (Guest or User) when they open the chat.
        /// Identifies the user (via Auth or DeviceId), finds/creates a session, and adds them to the group.
        /// </summary>
        /// <summary>
        /// Called by the Client (Guest or User) when they open the chat.
        /// Identifies the user (via Auth or DeviceId), finds/creates a session, and adds them to the group.
        /// Returns the Effective User ID used by the server.
        /// </summary>
        public async Task<string?> StartSession(string deviceId)
        {
            string userId = GetEffectiveUserId(deviceId);
            if (string.IsNullOrEmpty(userId)) return null!;

            // Find valid active session
            var session = await _context.LiveChatSessions
                .FirstOrDefaultAsync(s => (s.CustomerUserId == userId || s.GuestId == userId) && s.IsActive);

            if (session == null)
            {
                bool isAuthenticated = Context.User?.Identity?.IsAuthenticated == true;

                // Create new session if none exists
                session = new LiveChatSession
                {
                    StartTime = DateTime.UtcNow,
                    IsActive = true
                };

                if (isAuthenticated)
                {
                    session.CustomerUserId = userId;
                    session.CustomerUser = await _context.Users.FindAsync(userId);
                }
                else
                {
                    session.GuestId = userId;
                }
                _context.LiveChatSessions.Add(session);
                await _context.SaveChangesAsync();

                // Notify Moderators of new session
                await Clients.Group("Moderators").SendAsync("NewSessionStarted", session.Id, userId);
            }

            // JOIN THE GROUP
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Session-{session.Id}");

            // Send history to caller (Optional, good UX)
            // var history = await _context.LiveChatMessages.Where(m => m.LiveChatSessionId == session.Id).OrderBy(m => m.Timestamp).ToListAsync();
            // await Clients.Caller.SendAsync("ReceiveHistory", history);

            return userId;
        }

        /// <summary>
        /// Called by the Client to send a message.
        /// </summary>
        public async Task SendMessage(string message, string deviceId)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            string userId = GetEffectiveUserId(deviceId);

            var session = await _context.LiveChatSessions
                .FirstOrDefaultAsync(s => (s.CustomerUserId == userId || s.GuestId == userId) && s.IsActive);

            if (session == null)
            {
                // Should have called StartSession first, but let's handle it gracefully
                await StartSession(deviceId);
                session = await _context.LiveChatSessions.FirstOrDefaultAsync(s => s.CustomerUserId == userId && s.IsActive);
                if (session == null) return; // Error
            }

            // Save to DB
            var chatMsg = new LiveChatMessage
            {
                LiveChatSessionId = session.Id,
                SenderId = userId,
                Message = message,
                Timestamp = DateTime.UtcNow
            };
            _context.LiveChatMessages.Add(chatMsg);
            await _context.SaveChangesAsync();

            // Broadcast to the Session Group (includes Customer and Moderators watching this session)
            await Clients.Group($"Session-{session.Id}").SendAsync("ReceiveMessage", userId, message);

            // Also notify Moderators group generally (for notification toast/count)
            await Clients.Group("Moderators").SendAsync("SessionUpdated", session.Id, message);
        }

        /// <summary>
        /// Called by Moderator to reply.
        /// </summary>
        public async Task SendReply(int sessionId, string message)
        {
            // Security: Only Mods/Admins
            if (Context.User == null || (!Context.User.IsInRole("Admin") && !Context.User.IsInRole("Moderator")))
                throw new HubException("Yetkisiz erişim. Lütfen tekrar giriş yapın.");

            var session = await _context.LiveChatSessions.FindAsync(sessionId);
            if (session == null || !session.IsActive)
                throw new HubException("Bu sohbet oturumu kapalı.");

            // Save
            var chatMsg = new LiveChatMessage
            {
                LiveChatSessionId = sessionId,
                SenderId = "Support", // Or Context.UserIdentifier
                Message = message,
                Timestamp = DateTime.UtcNow
            };
            _context.LiveChatMessages.Add(chatMsg);
            await _context.SaveChangesAsync();

            // Broadcast to Session Group
            await Clients.Group($"Session-{sessionId}").SendAsync("ReceiveMessage", "Support", message);
        }

        /// <summary>
        /// Called by Moderators to join the "Moderators" global group and specific session groups they view.
        /// </summary>
        public async Task JoinModeratorZone()
        {
            if (Context.User != null && (Context.User.IsInRole("Admin") || Context.User.IsInRole("Moderator")))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Moderators");
            }
        }

        public async Task JoinSessionAsModerator(int sessionId)
        {
            if (Context.User != null && (Context.User.IsInRole("Admin") || Context.User.IsInRole("Moderator")))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Session-{sessionId}");
            }
        }

        private string GetEffectiveUserId(string deviceId)
        {
            // If logged in, use UserIdentifier. Else use provided deviceId (guestId).
            if (Context.User != null && Context.User.Identity != null && Context.User.Identity.IsAuthenticated)
            {
                return Context.UserIdentifier ?? deviceId;
            }
            return deviceId;
        }
    }
}
