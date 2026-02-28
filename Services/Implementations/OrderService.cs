using AutoMapper;
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

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, ICartService cartService, IHubContext<OrderHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cartService = cartService;
            _hubContext = hubContext;
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

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            // Clear cart
            await _cartService.ClearCartAsync(customerId);

            // Notify Admins via SignalR
            // Notify only Admins via SignalR (not all clients)
            await _hubContext.Clients.Group("Admins").SendAsync("ReceiveOrderNotification", order.OrderNumber, order.TotalAmount);

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null) return false;

            order.Status = status;
            
            if (status == OrderStatus.Shipped)
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
