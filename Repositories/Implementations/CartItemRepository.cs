using Microsoft.EntityFrameworkCore;
using StoreWave.Data;
using StoreWave.Models.Entities;
using StoreWave.Repositories.Interfaces;

namespace StoreWave.Repositories.Implementations
{
    public class CartItemRepository : Repository<CartItem>, ICartItemRepository
    {
        public CartItemRepository(ShopDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<CartItem>> GetCartItemsByCustomerAsync(int customerId)
        {
            return await _dbSet
                .Where(c => c.CustomerId == customerId)
                .Include(c => c.Product)
                .ToListAsync();
        }

        public async Task<CartItem?> GetCartItemAsync(int customerId, int productId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.CustomerId == customerId && c.ProductId == productId);
        }

        public async Task ClearCartAsync(int customerId)
        {
            var items = await _dbSet.Where(c => c.CustomerId == customerId).ToListAsync();
            if (items.Any())
            {
                _dbSet.RemoveRange(items);
            }
        }

        public async Task<int> GetCartItemCountAsync(int customerId)
        {
            return await _dbSet
                .Where(c => c.CustomerId == customerId)
                .SumAsync(c => c.Quantity);
        }
    }
}
