using CustomerService.Data;
using CustomerService.Models;
using CustomerService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Services
{
    public class CustomerServices : ICustomerService
    {
        private readonly CustomerContext _context;
        // private readonly ConnectionFactory _factory;
        
        public CustomerServices(CustomerContext context) // ConnectionFactory factory 
        {
            _context = context;
            // _factory = factory;
        }
        
        // public Task SendCustomerCreatedEventAsync(Customer customer)
        // {
        //     using var connection = _factory.CreateConnection();
        //     using var channel = connection.CreateModel();

        //     var message = System.Text.Json.JsonSerializer.Serialize(customer);
        //     var body = Encoding.UTF8.GetBytes(message);

        //     channel.BasicPublish(exchange: "customer_exchange",
        //                          routingKey: "new_customer",
        //                          basicProperties: null,
        //                          body: body);

        //     return Task.CompletedTask;
        // }


        public Task CreateNewCustomerAsync(Customer customer)
        {
            _context.Customers.Add(customer);
            return _context.SaveChangesAsync();
        }

        public async Task<Customer> SearchCustomerByIdAsync(int id) 
        {
            var customer = await _context.Customers.FindAsync(id) ?? throw new Exception($"Client with ID {id} not found.");

            return customer;
        }
        
        public async Task<List<Customer>> GetAllCustomersAsync() 
        {
            var customers = await _context.Customers.ToListAsync();

            if (customers == null || customers.Count == 0)
            {
                throw new Exception("No customers found.");
            }

            return customers;
        }




    }
}

