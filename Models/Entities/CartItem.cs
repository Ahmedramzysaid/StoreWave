using System.ComponentModel.DataAnnotations.Schema;

namespace StoreWave.Models.Entities
{
    public class CartItem
    {
        public int Id { get; set; }

        public int Quantity { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        public int CustomerId { get; set; }
        public int ProductId { get; set; }

        // Navigation Properties
        public virtual Customer Customer { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;

        // Computed Properties
        [NotMapped]
        public decimal TotalPrice => Product?.CurrentPrice * Quantity ?? 0;
    }
}
