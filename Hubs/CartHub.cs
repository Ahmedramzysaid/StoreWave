using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace StoreWave.Hubs
{
    /// <summary>
    /// SignalR Hub for real-time shopping cart synchronization.
    /// Allows cart updates to be reflected across multiple devices/tabs for the same user.
    /// </summary>
    public class CartHub : Hub
    {
        private readonly ILogger<CartHub> _logger;

        public CartHub(ILogger<CartHub> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Extracts the authenticated user's ID from the connection context.
        /// </summary>
        private int GetAuthenticatedUserId()
        {
            var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                throw new HubException("Unable to determine authenticated user identity.");
            }
            return userId;
        }

        /// <summary>
        /// Validates that the provided customerId matches the authenticated user.
        /// </summary>
        private void ValidateCustomerId(int customerId)
        {
            var authenticatedUserId = GetAuthenticatedUserId();
            if (authenticatedUserId != customerId)
            {
                _logger.LogWarning("User {AuthUserId} attempted to impersonate Customer {CustomerId} in CartHub", 
                    authenticatedUserId, customerId);
                throw new HubException("You can only access your own cart.");
            }
        }

        /// <summary>
        /// Called when a new client connects to the hub.
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("Cart client connected: {ConnectionId}", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Called when a client disconnects from the hub.
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation("Cart client disconnected: {ConnectionId}", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Join the user's cart synchronization group.
        /// This allows cart updates to sync across multiple browser tabs/devices.
        /// Validates that the customerId matches the authenticated user.
        /// </summary>
        /// <param name="customerId">The customer's ID</param>
        [Authorize]
        public async Task JoinCartGroup(int customerId)
        {
            ValidateCustomerId(customerId);
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Cart_{customerId}");
            _logger.LogInformation("Customer {CustomerId} joined cart sync group from {ConnectionId}", 
                customerId, Context.ConnectionId);
        }

        /// <summary>
        /// Leave the user's cart synchronization group.
        /// </summary>
        /// <param name="customerId">The customer's ID</param>
        [Authorize]
        public async Task LeaveCartGroup(int customerId)
        {
            ValidateCustomerId(customerId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Cart_{customerId}");
            _logger.LogInformation("Customer {CustomerId} left cart sync group from {ConnectionId}", 
                customerId, Context.ConnectionId);
        }

        /// <summary>
        /// Notify other connected clients that an item was added to the cart.
        /// Called from client after successful cart addition.
        /// Validates that the customerId matches the authenticated user.
        /// </summary>
        [Authorize]
        public async Task NotifyItemAdded(int customerId, int productId, string productName, int quantity, int newCartCount)
        {
            ValidateCustomerId(customerId);
            await Clients.OthersInGroup($"Cart_{customerId}").SendAsync("CartItemAdded", new
            {
                ProductId = productId,
                ProductName = productName,
                Quantity = quantity,
                NewCartCount = newCartCount,
                Timestamp = DateTime.UtcNow
            });
            _logger.LogInformation("Cart item added notification sent for customer {CustomerId}: {ProductName} x{Quantity}", 
                customerId, productName, quantity);
        }

        /// <summary>
        /// Notify other connected clients that an item was removed from the cart.
        /// Validates that the customerId matches the authenticated user.
        /// </summary>
        [Authorize]
        public async Task NotifyItemRemoved(int customerId, int productId, int newCartCount)
        {
            ValidateCustomerId(customerId);
            await Clients.OthersInGroup($"Cart_{customerId}").SendAsync("CartItemRemoved", new
            {
                ProductId = productId,
                NewCartCount = newCartCount,
                Timestamp = DateTime.UtcNow
            });
            _logger.LogInformation("Cart item removed notification sent for customer {CustomerId}: ProductId {ProductId}", 
                customerId, productId);
        }

        /// <summary>
        /// Notify other connected clients that the cart quantity was updated.
        /// Validates that the customerId matches the authenticated user.
        /// </summary>
        [Authorize]
        public async Task NotifyQuantityUpdated(int customerId, int productId, int newQuantity, int newCartCount)
        {
            ValidateCustomerId(customerId);
            await Clients.OthersInGroup($"Cart_{customerId}").SendAsync("CartQuantityUpdated", new
            {
                ProductId = productId,
                NewQuantity = newQuantity,
                NewCartCount = newCartCount,
                Timestamp = DateTime.UtcNow
            });
            _logger.LogInformation("Cart quantity updated notification sent for customer {CustomerId}: ProductId {ProductId}, Qty {NewQuantity}", 
                customerId, productId, newQuantity);
        }

        /// <summary>
        /// Notify other connected clients that the cart was cleared.
        /// Validates that the customerId matches the authenticated user.
        /// </summary>
        [Authorize]
        public async Task NotifyCartCleared(int customerId)
        {
            ValidateCustomerId(customerId);
            await Clients.OthersInGroup($"Cart_{customerId}").SendAsync("CartCleared", new
            {
                Timestamp = DateTime.UtcNow
            });
            _logger.LogInformation("Cart cleared notification sent for customer {CustomerId}", customerId);
        }

        /// <summary>
        /// Request a full cart sync from the server.
        /// Useful when a new tab/device connects and needs current cart state.
        /// Validates that the customerId matches the authenticated user.
        /// </summary>
        [Authorize]
        public async Task RequestCartSync(int customerId)
        {
            ValidateCustomerId(customerId);
            await Clients.Caller.SendAsync("CartSyncRequired", new
            {
                CustomerId = customerId,
                Timestamp = DateTime.UtcNow
            });
            _logger.LogInformation("Cart sync requested for customer {CustomerId}", customerId);
        }

        #region Server-to-Client Methods (called via IHubContext)

        // These methods are invoked from services using IHubContext<CartHub>
        // Client-side handlers:
        // - CartItemAdded(object data) - { ProductId, ProductName, Quantity, NewCartCount, Timestamp }
        // - CartItemRemoved(object data) - { ProductId, NewCartCount, Timestamp }
        // - CartQuantityUpdated(object data) - { ProductId, NewQuantity, NewCartCount, Timestamp }
        // - CartCleared(object data) - { Timestamp }
        // - CartSyncRequired(object data) - { CustomerId, Timestamp }
        // - CartUpdated(object data) - Full cart update from server

        #endregion
    }
}
