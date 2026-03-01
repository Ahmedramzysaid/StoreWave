using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StoreWave.Models.Entities;
using StoreWave.Services.Interfaces;

namespace StoreWave.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IChatService _chatService;
        private readonly UserManager<Customer> _userManager;

        public OrdersController(IOrderService orderService, IChatService chatService, UserManager<Customer> userManager)
        {
            _orderService = orderService;
            _chatService = chatService;
            _userManager = userManager;
        }

        private async Task<int> GetCurrentUserIdAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return user?.Id ?? 0;
        }

        public async Task<IActionResult> Index()
        {
            var userId = await GetCurrentUserIdAsync();
            var orders = await _orderService.GetOrdersByCustomerAsync(userId);
            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = await GetCurrentUserIdAsync();
            var order = await _orderService.GetOrderByIdAsync(id);
            
            if (order == null || order.CustomerId != userId)
            {
                return NotFound();
            }

            // Load existing chat messages so the customer can see previous conversations
            var chatMessages = await _chatService.GetMessagesForOrderAsync(id);
            ViewBag.ChatMessages = chatMessages;
            ViewBag.CurrentUserId = userId;

            return View(order);
        }
    }
}
