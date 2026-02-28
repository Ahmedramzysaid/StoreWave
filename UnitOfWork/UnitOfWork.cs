using StoreWave.Data;
using StoreWave.Repositories.Implementations;
using StoreWave.Repositories.Interfaces;

namespace StoreWave.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ShopDbContext _context;
        private IProductRepository? _products;
        private ICategoryRepository? _categories;
        private ICustomerRepository? _customers;
        private IOrderRepository? _orders;
        private ICartItemRepository? _cartItems;
        private IReviewRepository? _reviews;

        public UnitOfWork(ShopDbContext context)
        {
            _context = context;
        }

        public IProductRepository Products => _products ??= new ProductRepository(_context);
        public ICategoryRepository Categories => _categories ??= new CategoryRepository(_context);
        public ICustomerRepository Customers => _customers ??= new CustomerRepository(_context);
        public IOrderRepository Orders => _orders ??= new OrderRepository(_context);
        public ICartItemRepository CartItems => _cartItems ??= new CartItemRepository(_context);
        public IReviewRepository Reviews => _reviews ??= new ReviewRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
