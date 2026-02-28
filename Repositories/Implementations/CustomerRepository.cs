using Microsoft.EntityFrameworkCore;
using StoreWave.Data;
using StoreWave.Models.Entities;
using StoreWave.Repositories.Interfaces;

namespace StoreWave.Repositories.Implementations
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(ShopDbContext context) : base(context)
        {
        }

        public async Task<Customer?> GetCustomerByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<Customer?> GetCustomerWithOrdersAsync(int id)
        {
            return await _dbSet
                .Include(c => c.Orders)
                .ThenInclude(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Customer>> GetActiveCustomersAsync()
        {
            return await _dbSet
                .Where(c => c.IsActive)
                .OrderBy(c => c.LastName)
                .ThenBy(c => c.FirstName)
                .ToListAsync();
        }
    }
}
