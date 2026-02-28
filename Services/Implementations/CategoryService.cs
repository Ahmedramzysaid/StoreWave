using AutoMapper;
using StoreWave.DTOs;
using StoreWave.Models.Entities;
using StoreWave.Services.Interfaces;
using StoreWave.UnitOfWork;

namespace StoreWave.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync()
        {
            var categories = await _unitOfWork.Categories.GetActiveCategoriesAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetCategoryWithProductsAsync(id);
            return category == null ? null : _mapper.Map<CategoryDto>(category);
        }

        public async Task<bool> CreateCategoryAsync(CategoryDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            await _unitOfWork.Categories.AddAsync(category);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateCategoryAsync(CategoryDto categoryDto)
        {
            var existingCategory = await _unitOfWork.Categories.GetByIdAsync(categoryDto.Id);
            if (existingCategory == null) return false;

            _mapper.Map(categoryDto, existingCategory);
            _unitOfWork.Categories.Update(existingCategory);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null) return false;

            // Check if category has products
            if (await _unitOfWork.Categories.HasProductsAsync(id))
            {
                // Cannot delete category with products, verify logic via UI
                return false; 
            }

            _unitOfWork.Categories.Delete(category);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _unitOfWork.Categories.ExistsAsync(id);
        }
    }
}
