using System.ComponentModel.DataAnnotations;

namespace StoreWave.DTOs
{
    public class CartItemDto
    {
        public int Id { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
        public int Quantity { get; set; }

        public int CustomerId { get; set; }
        public int ProductId { get; set; }

        // Display properties
        [Display(Name = "Product")]
        public string? ProductName { get; set; }

        public string? ProductImageUrl { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Total")]
        public decimal TotalPrice { get; set; }

        public int StockQuantity { get; set; }
    }

    public class CartDto
    {
        public List<CartItemDto> Items { get; set; } = new();

        [DataType(DataType.Currency)]
        public decimal SubTotal { get; set; }

        [DataType(DataType.Currency)]
        public decimal ShippingCost { get; set; } = 10.00m;

        [DataType(DataType.Currency)]
        public decimal Total => SubTotal + ShippingCost;

        public int TotalItems => Items.Sum(i => i.Quantity);
    }
}
