using Microsoft.EntityFrameworkCore;
using StoreWave.Data;
using StoreWave.Models.Entities;
using StoreWave.Repositories.Interfaces;

namespace StoreWave.Repositories.Implementations
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ShopDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _dbSet
                .Where(p => p.CategoryId == categoryId && p.IsActive)
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetFeaturedProductsAsync(int count = 8)
        {
            return await _dbSet
                .Where(p => p.IsFeatured && p.IsActive)
                .Include(p => p.Category)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsOnSaleAsync()
        {
            return await _dbSet
                .Where(p => p.DiscountPrice.HasValue && p.DiscountPrice < p.Price && p.IsActive)
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetActiveProductsAsync();

            return await _dbSet
                .Where(p => p.IsActive && (p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm)))
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<Product?> GetProductWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .ThenInclude(r => r.Customer)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> GetActiveProductsAsync()
        {
            return await _dbSet
                .Where(p => p.IsActive)
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task UpdateStockAsync(int productId, int quantity)
        {
            var product = await GetByIdAsync(productId);
            if (product != null)
            {
                product.StockQuantity = quantity;
                // Note: SaveChanges is handled by UnitOfWork, but we mark it as modified here
                Update(product);
            }
        }
    }
}
