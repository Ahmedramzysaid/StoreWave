using StoreWave.DTOs;
using StoreWave.Models.Enums;

namespace StoreWave.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetOrdersByCustomerAsync(int customerId);
        Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(OrderStatus status);
        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<OrderDto?> GetOrderByNumberAsync(string orderNumber);
        Task<OrderDto> CreateOrderAsync(int customerId, OrderDto orderDto);
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status);
        Task<bool> AssignDriverAsync(int orderId, int driverId);
        Task<IEnumerable<OrderDto>> GetOrdersByDriverAsync(int driverId);
        Task<IEnumerable<OrderDto>> GetRecentOrdersAsync();
        Task<decimal> GetTotalSalesAsync();
        Task<int> GetTotalOrdersAsync();
    }
}
