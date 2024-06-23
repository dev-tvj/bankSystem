using CustomerService.Models;

namespace CustomerService.Services.Interfaces
{
    public interface ICustomerService
    {
        // Task SendCustomerCreatedEventAsync(Customer customer);
        Task CreateNewCustomerAsync(Customer customer);
        Task<Customer> SearchCustomerByIdAsync(int id);
        Task<List<Customer>> GetAllCustomersAsync();
    }
}
