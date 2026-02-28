using AutoMapper;
using StoreWave.DTOs;
using StoreWave.Models.Entities;
using StoreWave.Services.Interfaces;
using StoreWave.UnitOfWork;

namespace StoreWave.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _unitOfWork.Products.GetActiveProductsAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _unitOfWork.Products.GetProductsByCategoryAsync(categoryId);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm)
        {
            var products = await _unitOfWork.Products.SearchProductsAsync(searchTerm);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<IEnumerable<ProductDto>> GetFeaturedProductsAsync()
        {
            var products = await _unitOfWork.Products.GetFeaturedProductsAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsOnSaleAsync()
        {
            var products = await _unitOfWork.Products.GetProductsOnSaleAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetProductWithDetailsAsync(id);
            return product == null ? null : _mapper.Map<ProductDto>(product);
        }

        public async Task<bool> CreateProductAsync(ProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            product.CreatedAt = DateTime.UtcNow;
            
            await _unitOfWork.Products.AddAsync(product);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateProductAsync(ProductDto productDto)
        {
            var existingProduct = await _unitOfWork.Products.GetByIdAsync(productDto.Id);
            if (existingProduct == null) return false;

            _mapper.Map(productDto, existingProduct);
            existingProduct.UpdatedAt = DateTime.UtcNow;
            
            _unitOfWork.Products.Update(existingProduct);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null) return false;

            // Soft delete by setting IsActive to false
            product.IsActive = false;
            _unitOfWork.Products.Update(product);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _unitOfWork.Products.ExistsAsync(id);
        }
    }
}
