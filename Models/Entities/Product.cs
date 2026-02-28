using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreWave.Models.Entities
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? DiscountPrice { get; set; }

        public int StockQuantity { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsFeatured { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Foreign Keys
        public int CategoryId { get; set; }
        public int? SupplierId { get; set; }

        // Navigation Properties
        public virtual Category Category { get; set; } = null!;
        public virtual Customer? Supplier { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        // Computed Properties
        [NotMapped]
        public decimal CurrentPrice => DiscountPrice ?? Price;

        [NotMapped]
        public bool IsOnSale => DiscountPrice.HasValue && DiscountPrice < Price;

        [NotMapped]
        public int DiscountPercentage => IsOnSale 
            ? (int)Math.Round((1 - (DiscountPrice!.Value / Price)) * 100) 
            : 0;
    }
}
