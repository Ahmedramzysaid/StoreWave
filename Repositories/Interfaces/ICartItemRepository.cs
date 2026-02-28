using StoreWave.Models.Entities;

namespace StoreWave.Repositories.Interfaces
{
    public interface ICartItemRepository : IRepository<CartItem>
    {
        Task<IEnumerable<CartItem>> GetCartItemsByCustomerAsync(int customerId);
        Task<CartItem?> GetCartItemAsync(int customerId, int productId);
        Task ClearCartAsync(int customerId);
        Task<int> GetCartItemCountAsync(int customerId);
    }
}
