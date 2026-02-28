using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StoreWave.DTOs;
using StoreWave.Models.Entities;
using StoreWave.Models.Enums;
using StoreWave.Services.Interfaces;
using StoreWave.ViewModels;
using System.Text.Json;

namespace StoreWave.Controllers
{
    /// <summary>
    /// Controller for handling PayPal payment flow
    /// </summary>
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IPayPalService _payPalService;
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly UserManager<Customer> _userManager;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(
            IPayPalService payPalService,
            IOrderService orderService,
            ICartService cartService,
            UserManager<Customer> userManager,
            ILogger<PaymentController> logger)
        {
            _payPalService = payPalService;
            _orderService = orderService;
            _cartService = cartService;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Initiates PayPal checkout - creates order and redirects to PayPal
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePayPalOrder([FromBody] CheckoutViewModel checkoutModel)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var cartDetails = await _cartService.GetCartAsync(user.Id);
            if (cartDetails == null || !cartDetails.Items.Any())
            {
                return BadRequest(new { error = "Cart is empty" });
            }

            // Calculate total
            var total = cartDetails.Items.Sum(i => i.TotalPrice);

            // Store checkout data in session for later use
            var checkoutData = new
            {
                ShippingAddress = checkoutModel.ShippingAddress,
                ShippingCity = checkoutModel.ShippingCity,
                ShippingCountry = checkoutModel.ShippingCountry,
                ShippingPostalCode = checkoutModel.ShippingPostalCode,
                Notes = checkoutModel.Notes,
                UserId = user.Id,
                Total = total
            };
            HttpContext.Session.SetString("PayPalCheckoutData", JsonSerializer.Serialize(checkoutData));

            var request = new CreatePayPalOrderDto
            {
                Amount = total,
                Currency = "USD",
                Description = $"StoreWave Order - {cartDetails.Items.Count} items",
                ReturnUrl = Url.Action("PayPalSuccess", "Payment", null, Request.Scheme)!,
                CancelUrl = Url.Action("PayPalCancel", "Payment", null, Request.Scheme)!
            };

            var result = await _payPalService.CreateOrderAsync(request);

            if (!result.Success)
            {
                _logger.LogError("PayPal order creation failed: {Error}", result.ErrorMessage);
                return BadRequest(new { error = result.ErrorMessage });
            }

            // Store PayPal order ID in session
            HttpContext.Session.SetString("PayPalOrderId", result.OrderId);

            return Ok(new { approvalUrl = result.ApprovalUrl, orderId = result.OrderId });
        }

        /// <summary>
        /// Called when user returns from PayPal after approving payment
        /// </summary>
        public async Task<IActionResult> PayPalSuccess(string token, string PayerID)
        {
            var paypalOrderId = HttpContext.Session.GetString("PayPalOrderId");
            if (string.IsNullOrEmpty(paypalOrderId))
            {
                TempData["Error"] = "Payment session expired. Please try again.";
                return RedirectToAction("Checkout", "Cart");
            }

            // Capture the payment
            var captureResult = await _payPalService.CaptureOrderAsync(paypalOrderId);

            if (!captureResult.Success)
            {
                _logger.LogError("PayPal capture failed: {Error}", captureResult.ErrorMessage);
                TempData["Error"] = "Payment could not be completed. Please try again.";
                return RedirectToAction("Checkout", "Cart");
            }

            // Get checkout data from session
            var checkoutDataJson = HttpContext.Session.GetString("PayPalCheckoutData");
            if (string.IsNullOrEmpty(checkoutDataJson))
            {
                TempData["Error"] = "Checkout data lost. Please try again.";
                return RedirectToAction("Checkout", "Cart");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var cartDetails = await _cartService.GetCartAsync(user.Id);

            // Create the order
            using var document = JsonDocument.Parse(checkoutDataJson);
            var checkoutData = document.RootElement;

            var orderDto = new OrderDto
            {
                CustomerId = user.Id,
                CustomerName = user.FullName,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Processing,
                PaymentMethod = PaymentMethod.PayPal,
                SubTotal = cartDetails.Items.Sum(i => i.TotalPrice),
                ShippingCost = 0,
                // Tax = 0, // Tax property removed from OrderDto
                TotalAmount = captureResult.Amount,
                ShippingAddress = checkoutData.GetProperty("ShippingAddress").GetString() ?? "",
                ShippingCity = checkoutData.GetProperty("ShippingCity").GetString() ?? "",
                ShippingCountry = checkoutData.GetProperty("ShippingCountry").GetString() ?? "",
                ShippingPostalCode = checkoutData.GetProperty("ShippingPostalCode").GetString() ?? "",
                Notes = $"PayPal Transaction: {captureResult.TransactionId}",
                OrderItems = cartDetails.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    ProductImageUrl = i.ProductImageUrl,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    TotalPrice = i.TotalPrice
                }).ToList()
            };

            var order = await _orderService.CreateOrderAsync(user.Id, orderDto);

            // Clear cart
            await _cartService.ClearCartAsync(user.Id);

            // Clear session
            HttpContext.Session.Remove("PayPalOrderId");
            HttpContext.Session.Remove("PayPalCheckoutData");

            TempData["Success"] = $"Payment successful! Your order #{order.OrderNumber} has been placed.";
            return RedirectToAction("Confirmation", "Cart", new { id = order.Id });
        }

        /// <summary>
        /// Called when user cancels payment at PayPal
        /// </summary>
        public IActionResult PayPalCancel()
        {
            HttpContext.Session.Remove("PayPalOrderId");
            HttpContext.Session.Remove("PayPalCheckoutData");

            TempData["Warning"] = "Payment was cancelled. Your cart items are still saved.";
            return RedirectToAction("Checkout", "Cart");
        }
    }
}
