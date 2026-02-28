using AutoMapper;
using StoreWave.DTOs;
using StoreWave.Models.Entities;
using StoreWave.Services.Interfaces;
using StoreWave.UnitOfWork;

namespace StoreWave.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
        {
            var customers = await _unitOfWork.Customers.GetAllAsync();
            return _mapper.Map<IEnumerable<CustomerDto>>(customers);
        }

        public async Task<CustomerDto?> GetCustomerByIdAsync(int id)
        {
            var customer = await _unitOfWork.Customers.GetCustomerWithOrdersAsync(id);
            return customer == null ? null : _mapper.Map<CustomerDto>(customer);
        }

        public async Task<CustomerDto?> GetCustomerByEmailAsync(string email)
        {
            var customer = await _unitOfWork.Customers.GetCustomerByEmailAsync(email);
            return customer == null ? null : _mapper.Map<CustomerDto>(customer);
        }

        public async Task<bool> CreateCustomerAsync(CustomerDto customerDto)
        {
            var customer = _mapper.Map<Customer>(customerDto);
            await _unitOfWork.Customers.AddAsync(customer);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateCustomerAsync(CustomerDto customerDto)
        {
            var existingCustomer = await _unitOfWork.Customers.GetByIdAsync(customerDto.Id);
            if (existingCustomer == null) return false;

            _mapper.Map(customerDto, existingCustomer);
            _unitOfWork.Customers.Update(existingCustomer);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer == null) return false;

            customer.IsActive = false;
            _unitOfWork.Customers.Update(customer);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }
    }
}
