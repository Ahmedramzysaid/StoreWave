using System.ComponentModel.DataAnnotations;
using StoreWave.DTOs;
using StoreWave.Models.Enums;

namespace StoreWave.ViewModels
{
    public class CheckoutViewModel
    {
        public CartDto Cart { get; set; } = new();

        [Required(ErrorMessage = "Shipping address is required")]
        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        [Display(Name = "Shipping Address")]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required")]
        [StringLength(100)]
        [Display(Name = "City")]
        public string ShippingCity { get; set; } = string.Empty;

        [Required(ErrorMessage = "Country is required")]
        [StringLength(100)]
        [Display(Name = "Country")]
        public string ShippingCountry { get; set; } = string.Empty;

        [StringLength(20)]
        [Display(Name = "Postal Code")]
        public string? ShippingPostalCode { get; set; }

        [Required(ErrorMessage = "Payment method is required")]
        [Display(Name = "Payment Method")]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CashOnDelivery;

        [StringLength(500)]
        [Display(Name = "Order Notes")]
        public string? Notes { get; set; }
    }
}
