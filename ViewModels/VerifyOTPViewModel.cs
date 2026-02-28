using System.ComponentModel.DataAnnotations;

namespace StoreWave.ViewModels
{
    public class VerifyOTPViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "OTP Code")]
        public string OTP { get; set; }
    }
}
