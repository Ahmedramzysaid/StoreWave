namespace StoreWave.ViewModels
{
    using StoreWave.DTOs;
    
    public class HomeViewModel
    {
        public List<ProductDto> FeaturedProducts { get; set; } = new();
        public List<ProductDto> OnSaleProducts { get; set; } = new();
        public List<CategoryDto> Categories { get; set; } = new();
    }
}
