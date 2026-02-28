using StoreWave.Models.Entities;

namespace StoreWave.Repositories.Interfaces
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<IEnumerable<Review>> GetReviewsByProductAsync(int productId);
        Task<IEnumerable<Review>> GetReviewsByCustomerAsync(int customerId);
        Task<double> GetAverageRatingAsync(int productId);
        Task<bool> HasCustomerReviewedProductAsync(int customerId, int productId);
        Task<IEnumerable<Review>> GetRecentReviewsAsync(int count = 10);
    }
}
