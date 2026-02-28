using AutoMapper;
using StoreWave.DTOs;
using StoreWave.Models.Entities;
using StoreWave.Services.Interfaces;
using StoreWave.UnitOfWork;

namespace StoreWave.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByProductAsync(int productId)
        {
            var reviews = await _unitOfWork.Reviews.GetReviewsByProductAsync(productId);
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<bool> AddReviewAsync(ReviewDto reviewDto)
        {
            var review = _mapper.Map<Review>(reviewDto);
            review.CreatedAt = DateTime.UtcNow;
            review.IsApproved = true; // Auto-approve for demo
            
            await _unitOfWork.Reviews.AddAsync(review);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> ApproveReviewAsync(int id)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(id);
            if (review == null) return false;

            review.IsApproved = true;
            _unitOfWork.Reviews.Update(review);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteReviewAsync(int id)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(id);
            if (review == null) return false;

            _unitOfWork.Reviews.Delete(review);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<double> GetAverageRatingAsync(int productId)
        {
            return await _unitOfWork.Reviews.GetAverageRatingAsync(productId);
        }

        public async Task<bool> HasCustomerReviewedAsync(int customerId, int productId)
        {
            return await _unitOfWork.Reviews.HasCustomerReviewedProductAsync(customerId, productId);
        }
    }
}
