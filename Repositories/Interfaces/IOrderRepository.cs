using StoreWave.Models.Entities;
using StoreWave.Models.Enums;

namespace StoreWave.Repositories.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<Order?> GetOrderWithDetailsAsync(int id);
        Task<Order?> GetOrderByNumberAsync(string orderNumber);
        Task<IEnumerable<Order>> GetOrdersByCustomerAsync(int customerId);
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);
        Task<IEnumerable<Order>> GetRecentOrdersAsync(int count = 10);
        Task<decimal> GetTotalSalesAsync();
        Task<int> GetTotalOrdersAsync();
    }
}
