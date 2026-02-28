using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace StoreWave.Hubs
{
    /// <summary>
    /// SignalR Hub for general real-time notifications.
    /// Handles system alerts, low stock warnings, new reviews, and promotional notifications.
    /// </summary>
    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;

        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Called when a new client connects to the hub.
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("Notification client connected: {ConnectionId}", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Called when a client disconnects from the hub.
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation("Notification client disconnected: {ConnectionId}", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Allows admins to join the admin notifications group.
        /// </summary>
        [Authorize(Roles = "Admin")]
        public async Task JoinAdminNotifications()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "AdminNotifications");
            _logger.LogInformation("Admin {ConnectionId} joined AdminNotifications group", Context.ConnectionId);
            await Clients.Caller.SendAsync("NotificationReceived", new
            {
                Type = "info",
                Title = "Connected",
                Message = "You are now receiving admin notifications.",
                Timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Allows admins to leave the admin notifications group.
        /// </summary>
        [Authorize(Roles = "Admin")]
        public async Task LeaveAdminNotifications()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "AdminNotifications");
            _logger.LogInformation("Admin {ConnectionId} left AdminNotifications group", Context.ConnectionId);
        }

        /// <summary>
        /// Subscribe to notifications for a specific product (e.g., back-in-stock alerts).
        /// </summary>
        /// <param name="productId">The product ID to watch</param>
        [Authorize]
        public async Task WatchProduct(int productId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Product_{productId}");
            _logger.LogInformation("Client {ConnectionId} is watching product {ProductId}", 
                Context.ConnectionId, productId);
            await Clients.Caller.SendAsync("NotificationReceived", new
            {
                Type = "info",
                Title = "Product Watch",
                Message = "You will be notified when this product is back in stock.",
                Timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Unsubscribe from notifications for a specific product.
        /// </summary>
        /// <param name="productId">The product ID to stop watching</param>
        [Authorize]
        public async Task StopWatchingProduct(int productId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Product_{productId}");
            _logger.LogInformation("Client {ConnectionId} stopped watching product {ProductId}", 
                Context.ConnectionId, productId);
        }

        /// <summary>
        /// Subscribe to notifications for a specific category (e.g., new products, deals).
        /// </summary>
        /// <param name="categoryId">The category ID to subscribe to</param>
        [Authorize]
        public async Task SubscribeToCategory(int categoryId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Category_{categoryId}");
            _logger.LogInformation("Client {ConnectionId} subscribed to category {CategoryId}", 
                Context.ConnectionId, categoryId);
        }

        /// <summary>
        /// Unsubscribe from notifications for a specific category.
        /// </summary>
        /// <param name="categoryId">The category ID to unsubscribe from</param>
        [Authorize]
        public async Task UnsubscribeFromCategory(int categoryId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Category_{categoryId}");
            _logger.LogInformation("Client {ConnectionId} unsubscribed from category {CategoryId}", 
                Context.ConnectionId, categoryId);
        }

        /// <summary>
        /// Subscribe to promotional notifications.
        /// </summary>
        public async Task SubscribeToPromotions()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Promotions");
            _logger.LogInformation("Client {ConnectionId} subscribed to promotions", Context.ConnectionId);
        }

        /// <summary>
        /// Unsubscribe from promotional notifications.
        /// </summary>
        public async Task UnsubscribeFromPromotions()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Promotions");
            _logger.LogInformation("Client {ConnectionId} unsubscribed from promotions", Context.ConnectionId);
        }

        #region Server-to-Client Methods (called via IHubContext)

        // These methods are invoked from services using IHubContext<NotificationHub>
        // Client-side handlers:
        // - NotificationReceived(object notification) - General notification with Type, Title, Message, Timestamp
        // - LowStockAlert(int productId, string productName, int currentStock)
        // - NewReviewReceived(int productId, string productName, int rating, string customerName)
        // - BackInStock(int productId, string productName)
        // - NewProductInCategory(int categoryId, int productId, string productName)
        // - PromotionalAlert(string title, string message, string? couponCode)

        #endregion
    }
}
