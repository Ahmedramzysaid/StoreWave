using StoreWave.DTOs;

namespace StoreWave.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync();
        Task<CategoryDto?> GetCategoryByIdAsync(int id);
        Task<bool> CreateCategoryAsync(CategoryDto categoryDto);
        Task<bool> UpdateCategoryAsync(CategoryDto categoryDto);
        Task<bool> DeleteCategoryAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
