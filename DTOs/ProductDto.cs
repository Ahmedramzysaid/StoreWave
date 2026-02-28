using System.ComponentModel.DataAnnotations;

namespace StoreWave.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        [Display(Name = "Product Name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 999999.99, ErrorMessage = "Price must be between 0.01 and 999,999.99")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Range(0.01, 999999.99, ErrorMessage = "Discount price must be between 0.01 and 999,999.99")]
        [DataType(DataType.Currency)]
        [Display(Name = "Discount Price")]
        public decimal? DiscountPrice { get; set; }

        [Required(ErrorMessage = "Stock quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
        [Display(Name = "Stock Quantity")]
        public int StockQuantity { get; set; }

        [Display(Name = "Current Image")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Upload New Image")]
        public IFormFile? ImageFile { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Is Featured")]
        public bool IsFeatured { get; set; } = false;

        [Required(ErrorMessage = "Please select a category")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        // Display properties
        [Display(Name = "Category")]
        public string? CategoryName { get; set; }

        // Supplier info
        public int? SupplierId { get; set; }
        
        [Display(Name = "Supplier")]
        public string? SupplierName { get; set; }

        public decimal CurrentPrice { get; set; }
        public bool IsOnSale { get; set; }
        public int DiscountPercentage { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }

        // Reviews collection for display on product details page
        public List<ReviewDto> Reviews { get; set; } = new();
    }
}
