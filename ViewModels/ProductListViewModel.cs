using StoreWave.DTOs;

namespace StoreWave.ViewModels
{
    public class ProductListViewModel
    {
        public IEnumerable<ProductDto> Products { get; set; } = new List<ProductDto>();
        public IEnumerable<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
        public int? CurrentCategoryId { get; set; }
        public string? CurrentCategoryName { get; set; }
        public string? SearchString { get; set; }
    }
}
