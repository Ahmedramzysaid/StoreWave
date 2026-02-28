using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StoreWave.Models.Entities;
using StoreWave.Services.Interfaces;
using StoreWave.ViewModels;

namespace StoreWave.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Customer> _userManager;
        private readonly SignInManager<Customer> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _emailTemplateService;

        public AccountController(
            UserManager<Customer> userManager,
            SignInManager<Customer> signInManager,
            IEmailService emailService,
            IEmailTemplateService emailTemplateService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _emailTemplateService = emailTemplateService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new Customer
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    City = model.City,
                    Country = model.Country,
                    PostalCode = model.PostalCode,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Customer");
                    
                    // Send Welcome Email with professional template
                    var body = _emailTemplateService.WelcomeEmail(user.FirstName);
                    await _emailService.SendEmailAsync(user.Email!, "Welcome to StoreWave! 🎉", body);

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var model = new ProfileViewModel
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                City = user.City,
                Country = user.Country,
                PostalCode = user.PostalCode,
                Latitude = user.Latitude,
                Longitude = user.Longitude
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            user.City = model.City;
            user.Country = model.Country;
            user.PostalCode = model.PostalCode;
            user.Latitude = model.Latitude;
            user.Longitude = model.Longitude;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "Profile updated successfully!";
                return RedirectToAction("Profile");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    // Generate OTP
                    var otp = new Random().Next(100000, 999999).ToString();
                    
                    // Store OTP in Session
                    HttpContext.Session.SetString("OTP_" + model.Email, otp);
                    HttpContext.Session.SetString("OTP_Time_" + model.Email, DateTime.UtcNow.ToString());

                    // Send OTP Email with professional template
                    var body = _emailTemplateService.OtpEmail(user.FirstName, otp);
                    await _emailService.SendEmailAsync(model.Email, "🔐 Your StoreWave Password Reset Code", body);

                    return RedirectToAction("VerifyOTP", new { email = model.Email });
                }
                // Don't reveal that the user does not exist
                ModelState.AddModelError(string.Empty, "If an account exists, an OTP has been sent.");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult VerifyOTP(string email)
        {
            if (string.IsNullOrEmpty(email)) return RedirectToAction("ForgotPassword");
            return View(new VerifyOTPViewModel { Email = email });
        }

        [HttpPost]
        public IActionResult VerifyOTP(VerifyOTPViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var storedOtp = HttpContext.Session.GetString("OTP_" + model.Email);
            var storedTime = HttpContext.Session.GetString("OTP_Time_" + model.Email);

            if (storedOtp == model.OTP)
            {
                 // Check expiry (15 mins)
                 if (DateTime.TryParse(storedTime, out var time) && (DateTime.UtcNow - time).TotalMinutes < 15)
                 {
                     return RedirectToAction("ResetPassword", new { email = model.Email, otp = model.OTP });
                 }
                 ModelState.AddModelError(string.Empty, "OTP has expired.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid OTP.");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string otp)
        {
             if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(otp)) return RedirectToAction("ForgotPassword");
             return View(new ResetPasswordViewModel { Email = email, OTP = otp });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Re-verify OTP
            var storedOtp = HttpContext.Session.GetString("OTP_" + model.Email);
             if (storedOtp != model.OTP)
             {
                 ModelState.AddModelError(string.Empty, "Invalid OTP session.");
                 return View(model);
             }

             var user = await _userManager.FindByEmailAsync(model.Email);
             if (user != null)
             {
                 var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                 var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                 
                 if (result.Succeeded)
                 {
                     // Clear Session
                     HttpContext.Session.Remove("OTP_" + model.Email);
                     HttpContext.Session.Remove("OTP_Time_" + model.Email);

                     // Send password reset success confirmation email
                     var body = _emailTemplateService.PasswordResetSuccessEmail(user.FirstName);
                     await _emailService.SendEmailAsync(model.Email, "✅ Password Changed Successfully - StoreWave", body);

                     TempData["Success"] = "Password reset successfully! You can now login.";
                     return RedirectToAction("Login");
                 }
                 
                 foreach (var error in result.Errors)
                 {
                     ModelState.AddModelError(string.Empty, error.Description);
                 }
             }
             else
             {
                 ModelState.AddModelError(string.Empty, "User not found.");
             }

             return View(model);
        }
    }
}
