using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StoreWave.Models.Entities;
using StoreWave.Models.Enums;
using StoreWave.Services.Interfaces;
using StoreWave.ViewModels;

namespace StoreWave.Controllers
{
    /// <summary>
    /// Controller for InDriver Dashboard — delivery drivers pick up and deliver orders
    /// </summary>
    [Authorize(Roles = "InDriver")]
    public class InDriverController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly UserManager<Customer> _userManager;

        public InDriverController(
            IOrderService orderService,
            UserManager<Customer> userManager)
        {
            _orderService = orderService;
            _userManager = userManager;
        }

        // GET: InDriver
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var myOrders = await _orderService.GetOrdersByDriverAsync(user.Id);

            var viewModel = new InDriverDashboardViewModel
            {
                AssignedOrders = myOrders.Count(o => o.Status != OrderStatus.Delivered && o.Status != OrderStatus.Cancelled),
                DeliveredOrders = myOrders.Count(o => o.Status == OrderStatus.Delivered),
                InTransitOrders = myOrders.Count(o => o.Status == OrderStatus.OutForDelivery),
                AvailableOrders = myOrders.Count(o => o.Status == OrderStatus.Confirmed),
                ActiveOrders = myOrders
                    .Where(o => o.Status == OrderStatus.Confirmed || o.Status == OrderStatus.PickedUp || o.Status == OrderStatus.OutForDelivery)
                    .OrderByDescending(o => o.OrderDate)
                    .Take(10)
                    .ToList(),
                RecentDeliveries = myOrders
                    .Where(o => o.Status == OrderStatus.Delivered)
                    .OrderByDescending(o => o.DeliveredDate)
                    .Take(10)
                    .ToList()
            };

            return View(viewModel);
        }

        // GET: InDriver/Orders
        public async Task<IActionResult> Orders(string? filter)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var myOrders = await _orderService.GetOrdersByDriverAsync(user.Id);

            IEnumerable<DTOs.OrderDto> orders;

            switch (filter)
            {
                case "new":
                    orders = myOrders.Where(o => o.Status == OrderStatus.Confirmed);
                    break;
                case "active":
                    orders = myOrders.Where(o => o.Status == OrderStatus.PickedUp || o.Status == OrderStatus.OutForDelivery);
                    break;
                case "delivered":
                    orders = myOrders.Where(o => o.Status == OrderStatus.Delivered);
                    break;
                default: // "all"
                    orders = myOrders;
                    break;
            }

            ViewBag.CurrentFilter = filter ?? "all";
            return View(orders.OrderByDescending(o => o.OrderDate));
        }

        // GET: InDriver/OrderDetails/5
        public async Task<IActionResult> OrderDetails(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();

            // Only allow viewing orders assigned to this driver
            if (order.DriverId != user.Id)
            {
                return Forbid();
            }

            // Allowed statuses for InDriver to set
            var allowedStatuses = new List<OrderStatus>();
            ViewBag.CanPickup = false;

            if (order.Status == OrderStatus.Confirmed)
            {
                // Order is auto-assigned but not picked up yet
                ViewBag.CanPickup = true;
            }
            else if (order.Status == OrderStatus.PickedUp)
            {
                allowedStatuses.Add(OrderStatus.OutForDelivery);
                allowedStatuses.Add(OrderStatus.Delivered);
            }
            else if (order.Status == OrderStatus.OutForDelivery)
            {
                allowedStatuses.Add(OrderStatus.Delivered);
            }

            ViewBag.AllowedStatuses = new SelectList(allowedStatuses);
            return View(order);
        }

        // POST: InDriver/PickupOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PickupOrder(int orderId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // Verify order is assigned to this driver
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null || order.DriverId != user.Id)
            {
                TempData["Error"] = "This order is not assigned to you.";
                return RedirectToAction(nameof(Orders));
            }

            var result = await _orderService.UpdateOrderStatusAsync(orderId, OrderStatus.PickedUp);

            if (result)
            {
                TempData["Success"] = "Order picked up successfully! You are now delivering this order. 🚚";
            }
            else
            {
                TempData["Error"] = "Failed to pick up order.";
            }

            return RedirectToAction(nameof(OrderDetails), new { id = orderId });
        }

        // POST: InDriver/UpdateOrderStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, OrderStatus status)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // Verify this order belongs to this driver
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null || order.DriverId != user.Id)
            {
                TempData["Error"] = "You can only update orders assigned to you.";
                return RedirectToAction(nameof(Orders));
            }

            // Only allow valid InDriver statuses
            if (status != OrderStatus.OutForDelivery && status != OrderStatus.Delivered)
            {
                TempData["Error"] = "Invalid status for delivery driver.";
                return RedirectToAction(nameof(OrderDetails), new { id = orderId });
            }

            var result = await _orderService.UpdateOrderStatusAsync(orderId, status);

            if (result)
            {
                var statusMsg = status == OrderStatus.Delivered
                    ? "Order marked as delivered! Great job! 🎉"
                    : "Order is now out for delivery.";
                TempData["Success"] = statusMsg;
            }
            else
            {
                TempData["Error"] = "Failed to update order status.";
            }

            return RedirectToAction(nameof(OrderDetails), new { id = orderId });
        }
    }
}
