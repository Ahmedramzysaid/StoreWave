using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using StoreWave.Services.Interfaces;

namespace StoreWave.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(IChatService chatService, ILogger<ChatHub> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        private int GetUserId()
        {
            var claim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out var id) ? id : 0;
        }

        private bool IsAdmin()
        {
            return Context.User?.IsInRole("Admin") ?? false;
        }

        /// <summary>
        /// Join the chat room for a specific order
        /// </summary>
        public async Task JoinOrderChat(int orderId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"OrderChat_{orderId}");
            _logger.LogInformation("User {UserId} joined chat for order {OrderId}", GetUserId(), orderId);
        }

        /// <summary>
        /// Leave the chat room for a specific order
        /// </summary>
        public async Task LeaveOrderChat(int orderId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"OrderChat_{orderId}");
        }

        /// <summary>
        /// Send a message in an order's chat
        /// </summary>
        public async Task SendMessage(int orderId, string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            var userId = GetUserId();
            var isAdmin = IsAdmin();

            var chatMessage = await _chatService.SendMessageAsync(orderId, userId, message.Trim(), isAdmin);

            // Broadcast to everyone in the order chat room
            await Clients.Group($"OrderChat_{orderId}").SendAsync("ReceiveMessage", new
            {
                id = chatMessage.Id,
                senderId = chatMessage.SenderId,
                senderName = chatMessage.Sender.FullName,
                message = chatMessage.Message,
                sentAt = chatMessage.SentAt.ToString("MMM dd, hh:mm tt"),
                isAdminReply = chatMessage.IsAdminReply
            });

            _logger.LogInformation("Message sent in order {OrderId} by user {UserId} (Admin: {IsAdmin})", orderId, userId, isAdmin);
        }

        /// <summary>
        /// Mark messages as read when user opens the chat
        /// </summary>
        public async Task MarkAsRead(int orderId)
        {
            var userId = GetUserId();
            await _chatService.MarkMessagesAsReadAsync(orderId, userId);

            await Clients.Group($"OrderChat_{orderId}").SendAsync("MessagesRead", new
            {
                orderId,
                readByUserId = userId
            });
        }
    }
}
