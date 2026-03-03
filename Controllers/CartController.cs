using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StoreWave.DTOs;
using StoreWave.Models.Entities;
using StoreWave.Models.Enums;
using StoreWave.Services.Interfaces;
using StoreWave.ViewModels;

namespace StoreWave.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly UserManager<Customer> _userManager;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _emailTemplateService;

        public CartController(
            ICartService cartService,
            IOrderService orderService,
            IProductService productService,
            UserManager<Customer> userManager,
            IEmailService emailService,
            IEmailTemplateService emailTemplateService)
        {
            _cartService = cartService;
            _orderService = orderService;
            _productService = productService;
            _userManager = userManager;
            _emailService = emailService;
            _emailTemplateService = emailTemplateService;
        }

        private async Task<int> GetCurrentUserIdAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return user?.Id ?? 0;
        }

        public async Task<IActionResult> Index()
        {
            var userId = await GetCurrentUserIdAsync();
            var cart = await _cartService.GetCartAsync(userId);
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var userId = await GetCurrentUserIdAsync();
            await _cartService.AddItemToCartAsync(userId, productId, quantity);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var userId = await GetCurrentUserIdAsync();
            await _cartService.RemoveItemFromCartAsync(userId, productId);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {
            var userId = await GetCurrentUserIdAsync();
            await _cartService.UpdateItemQuantityAsync(userId, productId, quantity);
            return RedirectToAction(nameof(Index));
        }
        
        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            var userId = await GetCurrentUserIdAsync();
            var count = await _cartService.GetCartItemCountAsync(userId);
            return Json(new { count });
        }

        // GET: Cart/Checkout
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var userId = await GetCurrentUserIdAsync();
            var cart = await _cartService.GetCartAsync(userId);
            
            if (!cart.Items.Any())
            {
                TempData["Error"] = "Your cart is empty. Please add items before checkout.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.GetUserAsync(User);
            
            var viewModel = new CheckoutViewModel
            {
                Cart = cart,
                ShippingAddress = user?.Address ?? string.Empty,
                PaymentMethod = PaymentMethod.CashOnDelivery
            };

            return View(viewModel);
        }

        // POST: Cart/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var userId = await GetCurrentUserIdAsync();
            var cart = await _cartService.GetCartAsync(userId);
            
            if (!cart.Items.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                model.Cart = cart;
                return View(model);
            }

            try
            {
                var orderDto = new OrderDto
                {
                    ShippingAddress = model.ShippingAddress,
                    ShippingCity = model.ShippingCity,
                    ShippingCountry = model.ShippingCountry,
                    ShippingPostalCode = model.ShippingPostalCode,
                    PaymentMethod = model.PaymentMethod,
                    Notes = model.Notes
                };

                var createdOrder = await _orderService.CreateOrderAsync(userId, orderDto);
                
                TempData["OrderNumber"] = createdOrder.OrderNumber;

                var user = await _userManager.GetUserAsync(User);

                // Build item list for email templates
                var orderItems = createdOrder.OrderItems
                    .Select(i => (i.ProductName ?? "Product", i.Quantity, i.UnitPrice))
                    .ToList();

                // ─── 1. Customer: Order Confirmation ─────────────────────
                if (user != null)
                {
                    var customerBody = _emailTemplateService.OrderConfirmationEmail(
                        user.FirstName, createdOrder.OrderNumber, orderItems, createdOrder.TotalAmount);
                    await _emailService.SendEmailAsync(user.Email!, $"📦 Order Confirmed - {createdOrder.OrderNumber}", customerBody);
                }

                // ─── 2. Send role-based notifications (fire-and-forget style) ──
                await SendRoleBasedOrderNotificationsAsync(createdOrder, user, orderItems, model);

                return RedirectToAction(nameof(OrderSuccess));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while processing your order: " + ex.Message);
                model.Cart = cart;
                return View(model);
            }
        }

        /// <summary>
        /// Sends professional email notifications to Admin, Accountant, Supplier, and Warehouse users.
        /// </summary>
        private async Task SendRoleBasedOrderNotificationsAsync(
            OrderDto createdOrder, Customer? customer,
            List<(string ProductName, int Quantity, decimal Price)> orderItems,
            CheckoutViewModel model)
        {
            var customerName = customer?.FullName ?? "Customer";
            var shippingAddress = $"{model.ShippingAddress}, {model.ShippingCity}, {model.ShippingCountry} {model.ShippingPostalCode}";

            // ─── Admin Notifications ─────────────────────────────────
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            foreach (var admin in admins)
            {
                if (!string.IsNullOrEmpty(admin.Email))
                {
                    var body = _emailTemplateService.AdminNewOrderEmail(
                        createdOrder.OrderNumber, customerName, createdOrder.TotalAmount, createdOrder.OrderItems.Count);
                    await _emailService.SendEmailAsync(admin.Email, $"🔔 New Order - {createdOrder.OrderNumber}", body);
                }
            }

            // ─── Accountant Notifications ────────────────────────────
            var accountants = await _userManager.GetUsersInRoleAsync("Accountant");
            foreach (var accountant in accountants)
            {
                if (!string.IsNullOrEmpty(accountant.Email))
                {
                    var body = _emailTemplateService.AccountantOrderEmail(
                        createdOrder.OrderNumber, createdOrder.TotalAmount,
                        createdOrder.PaymentMethod.ToString(), customerName);
                    await _emailService.SendEmailAsync(accountant.Email, $"💰 New Transaction - {createdOrder.OrderNumber}", body);
                }
            }

            // ─── Warehouse Notifications ─────────────────────────────
            var warehouseManagers = await _userManager.GetUsersInRoleAsync("WarehouseManager");
            var warehouseItems = createdOrder.OrderItems
                .Select(i => (i.ProductName ?? "Product", i.Quantity))
                .ToList();
            foreach (var manager in warehouseManagers)
            {
                if (!string.IsNullOrEmpty(manager.Email))
                {
                    var body = _emailTemplateService.WarehouseOrderEmail(
                        createdOrder.OrderNumber, warehouseItems, shippingAddress);
                    await _emailService.SendEmailAsync(manager.Email, $"🏭 Order to Process - {createdOrder.OrderNumber}", body);
                }
            }

            // ─── Supplier Notifications (grouped by supplier) ────────
            var allProducts = await _productService.GetAllProductsAsync();
            var productMap = allProducts.ToDictionary(p => p.Id, p => p);

            // Group order items by supplier
            var supplierGroups = createdOrder.OrderItems
                .Where(item => productMap.ContainsKey(item.ProductId) && productMap[item.ProductId].SupplierId.HasValue)
                .GroupBy(item => productMap[item.ProductId].SupplierId!.Value);

            foreach (var group in supplierGroups)
            {
                var supplier = await _userManager.FindByIdAsync(group.Key.ToString());
                if (supplier != null && !string.IsNullOrEmpty(supplier.Email))
                {
                    var supplierItems = group
                        .Select(i => (i.ProductName ?? "Product", i.Quantity, i.UnitPrice))
                        .ToList();

                    var body = _emailTemplateService.SupplierOrderEmail(
                        supplier.FirstName, createdOrder.OrderNumber, supplierItems);
                    await _emailService.SendEmailAsync(supplier.Email, $"🏪 New Order for Your Products - {createdOrder.OrderNumber}", body);
                }
            }
        }

        // GET: Cart/OrderSuccess
        [HttpGet]
        public async Task<IActionResult> OrderSuccess()
        {
            var orderNumber = TempData["OrderNumber"]?.ToString();
            if (string.IsNullOrEmpty(orderNumber))
            {
                return RedirectToAction(nameof(Index));
            }

            var order = await _orderService.GetOrderByNumberAsync(orderNumber);
            if (order == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(order);
        }
    }
}
