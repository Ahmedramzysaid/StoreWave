using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StoreWave.DTOs;
using StoreWave.Models.Entities;
using StoreWave.Services.Interfaces;

namespace StoreWave.Controllers
{
    /// <summary>
    /// Controller for product reviews
    /// </summary>
    [Authorize]
    public class ReviewsController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly UserManager<Customer> _userManager;

        public ReviewsController(IReviewService reviewService, UserManager<Customer> userManager)
        {
            _reviewService = reviewService;
            _userManager = userManager;
        }

        private async Task<int> GetCurrentUserIdAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return user?.Id ?? 0;
        }

        // POST: Reviews/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReviewDto reviewDto)
        {
            var userId = await GetCurrentUserIdAsync();
            reviewDto.CustomerId = userId;
            reviewDto.CreatedAt = DateTime.UtcNow;
            reviewDto.IsApproved = true; // Auto-approve for simplicity

            // Check if user already reviewed this product
            var hasReviewed = await _reviewService.HasCustomerReviewedAsync(userId, reviewDto.ProductId);
            if (hasReviewed)
            {
                TempData["Error"] = "You have already reviewed this product.";
                return RedirectToAction("Details", "Products", new { id = reviewDto.ProductId });
            }

            if (reviewDto.Rating < 1 || reviewDto.Rating > 5)
            {
                TempData["Error"] = "Rating must be between 1 and 5.";
                return RedirectToAction("Details", "Products", new { id = reviewDto.ProductId });
            }

            var result = await _reviewService.AddReviewAsync(reviewDto);
            
            if (result)
            {
                TempData["Success"] = "Review submitted successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to submit review.";
            }

            return RedirectToAction("Details", "Products", new { id = reviewDto.ProductId });
        }

        // POST: Reviews/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int productId)
        {
            var result = await _reviewService.DeleteReviewAsync(id);
            
            if (result)
            {
                TempData["Success"] = "Review deleted.";
            }
            else
            {
                TempData["Error"] = "Failed to delete review.";
            }

            return RedirectToAction("Details", "Products", new { id = productId });
        }
    }
}
