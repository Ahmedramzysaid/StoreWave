using StoreWave.DTOs;

namespace StoreWave.Services.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDto>> GetReviewsByProductAsync(int productId);
        Task<bool> AddReviewAsync(ReviewDto reviewDto);
        Task<bool> ApproveReviewAsync(int id);
        Task<bool> DeleteReviewAsync(int id);
        Task<double> GetAverageRatingAsync(int productId);
        Task<bool> HasCustomerReviewedAsync(int customerId, int productId);
    }
}
