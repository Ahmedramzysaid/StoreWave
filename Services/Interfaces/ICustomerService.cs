using StoreWave.DTOs;

namespace StoreWave.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
        Task<CustomerDto?> GetCustomerByIdAsync(int id);
        Task<CustomerDto?> GetCustomerByEmailAsync(string email);
        Task<bool> CreateCustomerAsync(CustomerDto customerDto);
        Task<bool> UpdateCustomerAsync(CustomerDto customerDto);
        Task<bool> DeleteCustomerAsync(int id);
    }
}
