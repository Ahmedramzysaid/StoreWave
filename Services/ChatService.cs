using Microsoft.EntityFrameworkCore;
using StoreWave.Data;
using StoreWave.Models.Entities;
using StoreWave.Services.Interfaces;

namespace StoreWave.Services
{
    public class ChatService : IChatService
    {
        private readonly ShopDbContext _context;

        public ChatService(ShopDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChatMessage>> GetMessagesForOrderAsync(int orderId)
        {
            return await _context.ChatMessages
                .Include(m => m.Sender)
                .Where(m => m.OrderId == orderId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<ChatMessage> SendMessageAsync(int orderId, int senderId, string message, bool isAdminReply)
        {
            var chatMessage = new ChatMessage
            {
                OrderId = orderId,
                SenderId = senderId,
                Message = message,
                SentAt = DateTime.UtcNow,
                IsRead = false,
                IsAdminReply = isAdminReply
            };

            _context.ChatMessages.Add(chatMessage);
            await _context.SaveChangesAsync();

            // Load sender navigation property
            await _context.Entry(chatMessage).Reference(m => m.Sender).LoadAsync();

            return chatMessage;
        }

        public async Task MarkMessagesAsReadAsync(int orderId, int currentUserId)
        {
            // Mark messages NOT sent by the current user as read
            var unreadMessages = await _context.ChatMessages
                .Where(m => m.OrderId == orderId && m.SenderId != currentUserId && !m.IsRead)
                .ToListAsync();

            foreach (var msg in unreadMessages)
            {
                msg.IsRead = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<int> GetUnreadCountForAdminAsync()
        {
            // Count all unread messages from customers (non-admin replies)
            return await _context.ChatMessages
                .CountAsync(m => !m.IsRead && !m.IsAdminReply);
        }

        public async Task<List<OrderChatSummary>> GetOrdersWithChatsAsync()
        {
            var ordersWithChats = await _context.ChatMessages
                .Include(m => m.Order)
                    .ThenInclude(o => o.Customer)
                .GroupBy(m => m.OrderId)
                .Select(g => new OrderChatSummary
                {
                    OrderId = g.Key,
                    OrderNumber = g.First().Order.OrderNumber,
                    CustomerName = g.First().Order.Customer.FirstName + " " + g.First().Order.Customer.LastName,
                    LastMessage = g.OrderByDescending(m => m.SentAt).First().Message,
                    LastMessageTime = g.Max(m => m.SentAt),
                    UnreadCount = g.Count(m => !m.IsRead && !m.IsAdminReply)
                })
                .OrderByDescending(s => s.LastMessageTime)
                .ToListAsync();

            return ordersWithChats;
        }
    }
}
