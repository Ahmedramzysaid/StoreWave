using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StoreWave.Models.Enums;

namespace StoreWave.Models.Entities
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string OrderNumber { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingCost { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [StringLength(500)]
        public string? ShippingAddress { get; set; }

        [StringLength(100)]
        public string? ShippingCity { get; set; }

        [StringLength(100)]
        public string? ShippingCountry { get; set; }

        [StringLength(20)]
        public string? ShippingPostalCode { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime? ShippedDate { get; set; }

        public DateTime? DeliveredDate { get; set; }

        public DateTime? PickedUpDate { get; set; }

        // Foreign Keys
        public int CustomerId { get; set; }
        public int? DriverId { get; set; }

        // Navigation Properties
        public virtual Customer Customer { get; set; } = null!;
        public virtual Customer? Driver { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();

        // Helper method to generate order number
        public static string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        }
    }
}
