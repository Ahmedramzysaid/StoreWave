using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreWave.Models.Entities;
using StoreWave.Models.Enums;
using StoreWave.Services.Interfaces;
using StoreWave.ViewModels;

namespace StoreWave.Controllers
{
    /// <summary>
    /// Controller for Admin Dashboard and order management
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ICustomerService _customerService;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly UserManager<Customer> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly IChatService _chatService;

        public AdminController(
            IOrderService orderService,
            IProductService productService,
            ICategoryService categoryService,
            ICustomerService customerService,
            IEmailService emailService,
            IEmailTemplateService emailTemplateService,
            UserManager<Customer> userManager,
            RoleManager<IdentityRole<int>> roleManager,
            IChatService chatService)
        {
            _orderService = orderService;
            _productService = productService;
            _categoryService = categoryService;
            _customerService = customerService;
            _emailService = emailService;
            _emailTemplateService = emailTemplateService;
            _userManager = userManager;
            _roleManager = roleManager;
            _chatService = chatService;
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
            var totalOrders = await _orderService.GetTotalOrdersAsync();
            var totalSales = await _orderService.GetTotalSalesAsync();
            var recentOrders = await _orderService.GetRecentOrdersAsync();
            var products = await _productService.GetAllProductsAsync();
            var categories = await _categoryService.GetAllCategoriesAsync();
            var customers = await _customerService.GetAllCustomersAsync();

            var viewModel = new AdminDashboardViewModel
            {
                TotalOrders = totalOrders,
                TotalSales = totalSales,
                TotalProducts = products.Count(),
                TotalCategories = categories.Count(),
                TotalCustomers = customers.Count(),
                RecentOrders = recentOrders.Take(10).ToList(),
                PendingOrders = recentOrders.Count(o => o.Status == OrderStatus.Pending)
            };

            return View(viewModel);
        }

        // GET: Admin/Orders
        public async Task<IActionResult> Orders(OrderStatus? status)
        {
            IEnumerable<DTOs.OrderDto> orders;
            
            if (status.HasValue)
            {
                orders = await _orderService.GetOrdersByStatusAsync(status.Value);
            }
            else
            {
                orders = await _orderService.GetRecentOrdersAsync();
            }

            ViewBag.CurrentStatus = status;
            ViewBag.StatusList = new SelectList(Enum.GetValues<OrderStatus>(), status);
            
            return View(orders);
        }

        // GET: Admin/OrderDetails/5
        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            ViewBag.StatusList = new SelectList(Enum.GetValues<OrderStatus>(), order.Status);
            return View(order);
        }

        // POST: Admin/UpdateOrderStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, OrderStatus status)
        {
            var result = await _orderService.UpdateOrderStatusAsync(orderId, status);
            
            if (result)
            {
                TempData["Success"] = "Order status updated successfully.";

                // Send status change email to customer
                var order = await _orderService.GetOrderByIdAsync(orderId);
                if (order != null)
                {
                    var customer = await _userManager.FindByIdAsync(order.CustomerId.ToString());
                    if (customer != null && !string.IsNullOrEmpty(customer.Email))
                    {
                        var statusMessage = status switch
                        {
                            OrderStatus.Confirmed => "Your order has been confirmed and is being prepared. We'll update you when it ships!",
                            OrderStatus.Processing => "Your order is now being processed. Our team is working on getting it ready for shipment.",
                            OrderStatus.Shipped => "Great news! Your order has been shipped and is on its way to you. Keep an eye on your doorstep!",
                            OrderStatus.Delivered => "Your order has been delivered! We hope you enjoy your purchase. Don't forget to leave a review!",
                            OrderStatus.Cancelled => "Your order has been cancelled. If you have any questions, please contact our support team.",
                            _ => "Your order status has been updated."
                        };

                        var body = _emailTemplateService.OrderStatusEmail(
                            customer.FirstName, order.OrderNumber, status.ToString(), statusMessage);
                        await _emailService.SendEmailAsync(customer.Email, $"📋 Order {order.OrderNumber} - {status}", body);
                    }
                }
            }
            else
            {
                TempData["Error"] = "Failed to update order status.";
            }

            return RedirectToAction(nameof(OrderDetails), new { id = orderId });
        }

        // GET: Admin/Users
        public async Task<IActionResult> Users(string? searchTerm, string? roleFilter)
        {
            var allUsers = await _userManager.Users
                .Include(u => u.Orders)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            var allRoles = await _roleManager.Roles.Select(r => r.Name!).ToListAsync();

            var userProfiles = new List<AdminUserProfileViewModel>();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);

                // Apply role filter
                if (!string.IsNullOrEmpty(roleFilter) && !roles.Contains(roleFilter))
                    continue;

                var profile = new AdminUserProfileViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address,
                    City = user.City,
                    Country = user.Country,
                    PostalCode = user.PostalCode,
                    CreatedAt = user.CreatedAt,
                    IsActive = user.IsActive,
                    Roles = roles.ToList(),
                    TotalOrders = user.Orders.Count
                };

                // Apply search filter
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var term = searchTerm.ToLower();
                    if (!profile.FullName.ToLower().Contains(term) &&
                        !profile.Email.ToLower().Contains(term) &&
                        !(profile.PhoneNumber?.ToLower().Contains(term) ?? false))
                        continue;
                }

                userProfiles.Add(profile);
            }

            var viewModel = new AdminUsersListViewModel
            {
                Users = userProfiles,
                SearchTerm = searchTerm,
                RoleFilter = roleFilter,
                AvailableRoles = allRoles
            };

            return View(viewModel);
        }

        // GET: Admin/UserDetails/5
        public async Task<IActionResult> UserDetails(int id)
        {
            var user = await _userManager.Users
                .Include(u => u.Orders)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            var profile = new AdminUserProfileViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                City = user.City,
                Country = user.Country,
                PostalCode = user.PostalCode,
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive,
                Roles = roles.ToList(),
                TotalOrders = user.Orders.Count
            };

            return View(profile);
        }

        // GET: Admin/OrderChats
        public async Task<IActionResult> OrderChats()
        {
            var chatSummaries = await _chatService.GetOrdersWithChatsAsync();
            ViewBag.TotalUnread = await _chatService.GetUnreadCountForAdminAsync();
            return View(chatSummaries);
        }

        // GET: Admin/OrderChat/5
        public async Task<IActionResult> OrderChat(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();

            var messages = await _chatService.GetMessagesForOrderAsync(id);

            // Mark customer messages as read for admin
            var adminUser = await _userManager.GetUserAsync(User);
            if (adminUser != null)
            {
                await _chatService.MarkMessagesAsReadAsync(id, adminUser.Id);
            }

            ViewBag.Order = order;
            ViewBag.CurrentUserId = adminUser?.Id ?? 0;
            return View(messages);
        }
    }
}
