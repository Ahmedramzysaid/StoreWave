using StoreWave.DTOs;

namespace StoreWave.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(int customerId);
        Task AddItemToCartAsync(int customerId, int productId, int quantity);
        Task RemoveItemFromCartAsync(int customerId, int productId);
        Task UpdateItemQuantityAsync(int customerId, int productId, int quantity);
        Task ClearCartAsync(int customerId);
        Task<int> GetCartItemCountAsync(int customerId);
    }
}
