using StoreWave.Models.Entities;

namespace StoreWave.Repositories.Interfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer?> GetCustomerByEmailAsync(string email);
        Task<Customer?> GetCustomerWithOrdersAsync(int id);
        Task<IEnumerable<Customer>> GetActiveCustomersAsync();
    }
}
