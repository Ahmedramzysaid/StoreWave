using System.ComponentModel.DataAnnotations;

namespace StoreWave.Models.Entities
{
    public class ChatMessage
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int SenderId { get; set; }

        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = string.Empty;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;

        /// <summary>
        /// True if the sender is an Admin/Support, false if Customer
        /// </summary>
        public bool IsAdminReply { get; set; } = false;

        // Navigation Properties
        public virtual Order Order { get; set; } = null!;
        public virtual Customer Sender { get; set; } = null!;
    }
}
