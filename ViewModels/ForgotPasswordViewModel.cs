using System.ComponentModel.DataAnnotations;

namespace StoreWave.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
