using StoreWave.DTOs;

namespace StoreWave.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm);
        Task<IEnumerable<ProductDto>> GetFeaturedProductsAsync();
        Task<IEnumerable<ProductDto>> GetProductsOnSaleAsync();
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<bool> CreateProductAsync(ProductDto productDto);
        Task<bool> UpdateProductAsync(ProductDto productDto);
        Task<bool> DeleteProductAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
