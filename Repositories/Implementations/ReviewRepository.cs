using Microsoft.EntityFrameworkCore;
using StoreWave.Data;
using StoreWave.Models.Entities;
using StoreWave.Repositories.Interfaces;

namespace StoreWave.Repositories.Implementations
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        public ReviewRepository(ShopDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Review>> GetReviewsByProductAsync(int productId)
        {
            return await _dbSet
                .Where(r => r.ProductId == productId && r.IsApproved)
                .Include(r => r.Customer)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetReviewsByCustomerAsync(int customerId)
        {
            return await _dbSet
                .Where(r => r.CustomerId == customerId)
                .Include(r => r.Product)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<double> GetAverageRatingAsync(int productId)
        {
            var ratings = await _dbSet
                .Where(r => r.ProductId == productId && r.IsApproved)
                .Select(r => r.Rating)
                .ToListAsync();

            return ratings.Any() ? ratings.Average() : 0;
        }

        public async Task<bool> HasCustomerReviewedProductAsync(int customerId, int productId)
        {
            return await _dbSet.AnyAsync(r => r.CustomerId == customerId && r.ProductId == productId);
        }

        public async Task<IEnumerable<Review>> GetRecentReviewsAsync(int count = 10)
        {
            return await _dbSet
                .Include(r => r.Customer)
                .Include(r => r.Product)
                .OrderByDescending(r => r.CreatedAt)
                .Take(count)
                .ToListAsync();
        }
    }
}
