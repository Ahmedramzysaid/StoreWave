using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using StoreWave.Models.Enums;

namespace StoreWave.Hubs
{
    /// <summary>
    /// SignalR Hub for real-time order notifications and updates.
    /// Supports both admin-wide broadcasts and customer-specific order tracking.
    /// </summary>
    public class OrderHub : Hub
    {
        private readonly ILogger<OrderHub> _logger;

        public OrderHub(ILogger<OrderHub> logger)
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
                _logger.LogWarning("User {AuthUserId} attempted to impersonate Customer {CustomerId}", 
                    authenticatedUserId, customerId);
                throw new HubException("You can only access your own data.");
            }
        }

        /// <summary>
        /// Called when a new client connects to the hub.
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Called when a client disconnects from the hub.
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Allows admins to join the admin group for receiving all order notifications.
        /// </summary>
        [Authorize(Roles = "Admin")]
        public async Task JoinAdminGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
            _logger.LogInformation("Admin {ConnectionId} joined Admins group", Context.ConnectionId);
        }

        /// <summary>
        /// Allows admins to leave the admin group.
        /// </summary>
        [Authorize(Roles = "Admin")]
        public async Task LeaveAdminGroup()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Admins");
            _logger.LogInformation("Admin {ConnectionId} left Admins group", Context.ConnectionId);
        }

        /// <summary>
        /// Allows a customer to subscribe to updates for a specific order.
        /// </summary>
        /// <param name="orderNumber">The order number to track</param>
        [Authorize]
        public async Task TrackOrder(string orderNumber)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Order_{orderNumber}");
            _logger.LogInformation("Client {ConnectionId} is now tracking order {OrderNumber}", 
                Context.ConnectionId, orderNumber);
        }

        /// <summary>
        /// Allows a customer to unsubscribe from updates for a specific order.
        /// </summary>
        /// <param name="orderNumber">The order number to stop tracking</param>
        [Authorize]
        public async Task StopTrackingOrder(string orderNumber)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Order_{orderNumber}");
            _logger.LogInformation("Client {ConnectionId} stopped tracking order {OrderNumber}", 
                Context.ConnectionId, orderNumber);
        }

        /// <summary>
        /// Allows a customer to join their personal notification group.
        /// Validates that the customerId matches the authenticated user.
        /// </summary>
        /// <param name="customerId">The customer's ID</param>
        [Authorize]
        public async Task JoinCustomerGroup(int customerId)
        {
            ValidateCustomerId(customerId);
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Customer_{customerId}");
            _logger.LogInformation("Customer {CustomerId} joined their notification group", customerId);
        }

        /// <summary>
        /// Allows a customer to leave their personal notification group.
        /// Validates that the customerId matches the authenticated user.
        /// </summary>
        /// <param name="customerId">The customer's ID</param>
        [Authorize]
        public async Task LeaveCustomerGroup(int customerId)
        {
            ValidateCustomerId(customerId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Customer_{customerId}");
            _logger.LogInformation("Customer {CustomerId} left their notification group", customerId);
        }

        /// <summary>
        /// Allows an InDriver to join their personal driver notification group.
        /// </summary>
        [Authorize(Roles = "InDriver")]
        public async Task JoinDriverGroup()
        {
            var userId = GetAuthenticatedUserId();
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Driver_{userId}");
            _logger.LogInformation("Driver {DriverId} joined their notification group", userId);
        }

        /// <summary>
        /// Allows an InDriver to leave their personal driver notification group.
        /// </summary>
        [Authorize(Roles = "InDriver")]
        public async Task LeaveDriverGroup()
        {
            var userId = GetAuthenticatedUserId();
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Driver_{userId}");
            _logger.LogInformation("Driver {DriverId} left their notification group", userId);
        }

        #region Server-to-Client Methods (called via IHubContext)

        // These methods are invoked from services using IHubContext<OrderHub>
        // Client-side handlers:
        // - ReceiveOrderNotification(string orderNumber, decimal totalAmount)
        // - ReceiveOrderStatusUpdate(string orderNumber, string status, DateTime? shippedDate, DateTime? deliveredDate)
        // - ReceiveNewOrderForAdmin(OrderDto order)

        #endregion
    }
}
