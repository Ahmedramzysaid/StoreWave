using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace StoreWave.Models.Entities
{
    // Inherit from IdentityUser using int as the primary key type
    public class Customer : IdentityUser<int>
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        // Email, PhoneNumber, and Id are inherited from IdentityUser

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        [StringLength(20)]
        public string? PostalCode { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        
        // Supplier Products - Products listed by this user if they are a Supplier
        public virtual ICollection<Product> SupplierProducts { get; set; } = new List<Product>();

        // Computed Properties
        public string FullName => $"{FirstName} {LastName}";
    }
}
