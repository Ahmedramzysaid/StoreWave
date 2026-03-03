using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using StoreWave.DTOs;
using StoreWave.Hubs;
using StoreWave.Models.Entities;
using StoreWave.Models.Enums;
using StoreWave.Services.Interfaces;
using StoreWave.UnitOfWork;

namespace StoreWave.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICartService _cartService;
        private readonly IHubContext<OrderHub> _hubContext;
        private readonly UserManager<Customer> _userManager;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, ICartService cartService, 
            IHubContext<OrderHub> hubContext, UserManager<Customer> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cartService = cartService;
            _hubContext = hubContext;
            _userManager = userManager;
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByCustomerAsync(int customerId)
        {
            var orders = await _unitOfWork.Orders.GetOrdersByCustomerAsync(customerId);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(OrderStatus status)
        {
            var orders = await _unitOfWork.Orders.GetOrdersByStatusAsync(status);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            var order = await _unitOfWork.Orders.GetOrderWithDetailsAsync(id);
            return order == null ? null : _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto?> GetOrderByNumberAsync(string orderNumber)
        {
            var order = await _unitOfWork.Orders.GetOrderByNumberAsync(orderNumber);
            return order == null ? null : _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> CreateOrderAsync(int customerId, OrderDto orderDto)
        {
            // Get cart items
            var cart = await _cartService.GetCartAsync(customerId);
            if (!cart.Items.Any())
            {
                throw new InvalidOperationException("Cart is empty");
            }

            var order = new Order
            {
                OrderNumber = Order.GenerateOrderNumber(),
                CustomerId = customerId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                ShippingAddress = orderDto.ShippingAddress,
                ShippingCity = orderDto.ShippingCity,
                ShippingCountry = orderDto.ShippingCountry,
                ShippingPostalCode = orderDto.ShippingPostalCode,
                Notes = orderDto.Notes,
                PaymentMethod = orderDto.PaymentMethod,
                SubTotal = cart.SubTotal,
                ShippingCost = cart.ShippingCost,
                TotalAmount = cart.Total,
                OrderItems = new List<OrderItem>()
            };

            foreach (var item in cart.Items)
            {
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice
                });

                // Update stock
                await _unitOfWork.Products.UpdateStockAsync(item.ProductId, item.StockQuantity - item.Quantity);
            }

            // Auto-assign to driver with fewest active orders (round-robin)
            var nextDriver = await GetNextAvailableDriverAsync();
            if (nextDriver != null)
            {
                order.DriverId = nextDriver.Id;
                order.Status = OrderStatus.Confirmed;
            }

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            // Clear cart
            await _cartService.ClearCartAsync(customerId);

            // Notify Admins via SignalR
            await _hubContext.Clients.Group("Admins").SendAsync("ReceiveOrderNotification", order.OrderNumber, order.TotalAmount);

            // Notify assigned driver via SignalR
            if (order.DriverId.HasValue)
            {
                await _hubContext.Clients.Group($"Driver_{order.DriverId.Value}").SendAsync(
                    "ReceiveNewDelivery",
                    order.OrderNumber,
                    order.TotalAmount,
                    order.ShippingAddress ?? "",
                    order.ShippingCity ?? "");
            }

            return _mapper.Map<OrderDto>(order);
        }

        /// <summary>
        /// Finds the InDriver with the fewest active (non-delivered, non-cancelled) orders.
        /// This creates a round-robin effect — the least busy driver gets the next order.
        /// </summary>
        private async Task<Customer?> GetNextAvailableDriverAsync()
        {
            var drivers = await _userManager.GetUsersInRoleAsync("InDriver");
            if (!drivers.Any()) return null;

            // For each driver, count their active orders (not Delivered, not Cancelled)
            var driverWorkloads = new List<(Customer Driver, int ActiveCount)>();

            foreach (var driver in drivers)
            {
                var driverOrders = await _unitOfWork.Orders.GetOrdersByDriverAsync(driver.Id);
                var activeCount = driverOrders.Count(o => 
                    o.Status != OrderStatus.Delivered && o.Status != OrderStatus.Cancelled);
                driverWorkloads.Add((driver, activeCount));
            }

            // Pick the driver with the fewest active orders
            return driverWorkloads
                .OrderBy(d => d.ActiveCount)
                .ThenBy(d => d.Driver.Id) // tiebreaker: lowest ID first for consistency
                .First()
                .Driver;
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null) return false;

            order.Status = status;
            
            if (status == OrderStatus.PickedUp)
                order.PickedUpDate = DateTime.UtcNow;
            else if (status == OrderStatus.Shipped)
                order.ShippedDate = DateTime.UtcNow;
            else if (status == OrderStatus.Delivered)
                order.DeliveredDate = DateTime.UtcNow;

            _unitOfWork.Orders.Update(order);
            var result = await _unitOfWork.SaveChangesAsync() > 0;

            if (result)
            {
                // Notify the customer in real-time about their order status change
                await _hubContext.Clients.Group($"Customer_{order.CustomerId}").SendAsync(
                    "ReceiveOrderStatusUpdate",
                    order.OrderNumber,
                    status.ToString(),
                    order.ShippedDate,
                    order.DeliveredDate);

                // Also notify anyone tracking this specific order
                await _hubContext.Clients.Group($"Order_{order.OrderNumber}").SendAsync(
                    "ReceiveOrderStatusUpdate",
                    order.OrderNumber,
                    status.ToString(),
                    order.ShippedDate,
                    order.DeliveredDate);
            }

            return result;
        }

        public async Task<bool> AssignDriverAsync(int orderId, int driverId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null) return false;

            order.DriverId = driverId;
            order.Status = OrderStatus.PickedUp;
            order.PickedUpDate = DateTime.UtcNow;

            _unitOfWork.Orders.Update(order);
            var result = await _unitOfWork.SaveChangesAsync() > 0;

            if (result)
            {
                await _hubContext.Clients.Group($"Customer_{order.CustomerId}").SendAsync(
                    "ReceiveOrderStatusUpdate",
                    order.OrderNumber,
                    OrderStatus.PickedUp.ToString(),
                    order.ShippedDate,
                    order.DeliveredDate);
            }

            return result;
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByDriverAsync(int driverId)
        {
            var orders = await _unitOfWork.Orders.GetOrdersByDriverAsync(driverId);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<IEnumerable<OrderDto>> GetRecentOrdersAsync()
        {
            var orders = await _unitOfWork.Orders.GetRecentOrdersAsync();
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<decimal> GetTotalSalesAsync()
        {
            return await _unitOfWork.Orders.GetTotalSalesAsync();
        }

        public async Task<int> GetTotalOrdersAsync()
        {
            return await _unitOfWork.Orders.GetTotalOrdersAsync();
        }
    }
}
