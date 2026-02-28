using StoreWave.Models.Entities;

namespace StoreWave.Services.Interfaces
{
    public interface IChatService
    {
        Task<List<ChatMessage>> GetMessagesForOrderAsync(int orderId);
        Task<ChatMessage> SendMessageAsync(int orderId, int senderId, string message, bool isAdminReply);
        Task MarkMessagesAsReadAsync(int orderId, int currentUserId);
        Task<int> GetUnreadCountForAdminAsync();
        Task<List<OrderChatSummary>> GetOrdersWithChatsAsync();
    }

    /// <summary>
    /// Summary DTO for the admin chat list page
    /// </summary>
    public class OrderChatSummary
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string LastMessage { get; set; } = string.Empty;
        public DateTime LastMessageTime { get; set; }
        public int UnreadCount { get; set; }
    }
}
