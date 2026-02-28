using StoreWave.Repositories.Interfaces;

namespace StoreWave.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        ICategoryRepository Categories { get; }
        ICustomerRepository Customers { get; }
        IOrderRepository Orders { get; }
        ICartItemRepository CartItems { get; }
        IReviewRepository Reviews { get; }
        
        Task<int> SaveChangesAsync();
    }
}
