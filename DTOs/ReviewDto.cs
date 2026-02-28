using System.ComponentModel.DataAnnotations;

namespace StoreWave.DTOs
{
    public class ReviewDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string? Title { get; set; }

        [StringLength(2000, ErrorMessage = "Comment cannot exceed 2000 characters")]
        public string? Comment { get; set; }

        [Display(Name = "Created")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Approved")]
        public bool IsApproved { get; set; } = true;

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        // Display properties
        [Display(Name = "Product")]
        public string? ProductName { get; set; }

        [Display(Name = "Customer")]
        public string? CustomerName { get; set; }
    }
}
