using System.ComponentModel.DataAnnotations;
using StoreWave.Models.Enums;

namespace StoreWave.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }

        [Display(Name = "Order Number")]
        public string OrderNumber { get; set; } = string.Empty;

        [Display(Name = "Order Date")]
        [DataType(DataType.DateTime)]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Subtotal")]
        [DataType(DataType.Currency)]
        public decimal SubTotal { get; set; }

        [Display(Name = "Shipping Cost")]
        [DataType(DataType.Currency)]
        public decimal ShippingCost { get; set; }

        [Display(Name = "Total Amount")]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; }

        [Required(ErrorMessage = "Shipping address is required")]
        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        [Display(Name = "Shipping Address")]
        public string? ShippingAddress { get; set; }

        [StringLength(100)]
        [Display(Name = "City")]
        public string? ShippingCity { get; set; }

        [StringLength(100)]
        [Display(Name = "Country")]
        public string? ShippingCountry { get; set; }

        [StringLength(20)]
        [Display(Name = "Postal Code")]
        public string? ShippingPostalCode { get; set; }

        [Display(Name = "Payment Method")]
        public PaymentMethod PaymentMethod { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [Display(Name = "Shipped Date")]
        [DataType(DataType.DateTime)]
        public DateTime? ShippedDate { get; set; }

        [Display(Name = "Delivered Date")]
        [DataType(DataType.DateTime)]
        public DateTime? DeliveredDate { get; set; }

        [Display(Name = "Picked Up Date")]
        [DataType(DataType.DateTime)]
        public DateTime? PickedUpDate { get; set; }

        [Required(ErrorMessage = "Customer is required")]
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }

        [Display(Name = "Driver")]
        public int? DriverId { get; set; }

        // Display properties
        [Display(Name = "Customer")]
        public string? CustomerName { get; set; }

        [Display(Name = "Driver")]
        public string? DriverName { get; set; }

        [Display(Name = "Items")]
        public int ItemCount { get; set; }

        public List<OrderItemDto> OrderItems { get; set; } = new();
    }

    public class OrderItemDto
    {
        public int Id { get; set; }
        public int Quantity { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Total")]
        public decimal TotalPrice { get; set; }

        public int ProductId { get; set; }

        [Display(Name = "Product")]
        public string? ProductName { get; set; }

        public string? ProductImageUrl { get; set; }
    }
}
