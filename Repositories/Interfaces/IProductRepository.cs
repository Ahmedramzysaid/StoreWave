using StoreWave.Models.Entities;

namespace StoreWave.Repositories.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<Product>> GetFeaturedProductsAsync(int count = 8);
        Task<IEnumerable<Product>> GetProductsOnSaleAsync();
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
        Task<Product?> GetProductWithDetailsAsync(int id);
        Task<IEnumerable<Product>> GetActiveProductsAsync();
        Task UpdateStockAsync(int productId, int quantity);
    }
}
