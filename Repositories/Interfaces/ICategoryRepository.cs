using StoreWave.Models.Entities;

namespace StoreWave.Repositories.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IEnumerable<Category>> GetActiveCategoriesAsync();
        Task<Category?> GetCategoryWithProductsAsync(int id);
        Task<bool> HasProductsAsync(int categoryId);
    }
}
