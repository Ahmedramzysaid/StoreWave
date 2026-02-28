using System.ComponentModel.DataAnnotations;

namespace StoreWave.Models.Entities
{
    public class Review
    {
        public int Id { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(100)]
        public string? Title { get; set; }

        [StringLength(2000)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsApproved { get; set; } = true;

        // Foreign Keys
        public int ProductId { get; set; }
        public int CustomerId { get; set; }

        // Navigation Properties
        public virtual Product Product { get; set; } = null!;
        public virtual Customer Customer { get; set; } = null!;
    }
}
