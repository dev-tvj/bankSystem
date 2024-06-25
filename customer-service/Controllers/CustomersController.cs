using CustomerService.Models;
using CustomerService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace CustomerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerServices _customerServices;

        public CustomersController(ICustomerServices customerServices)
        {
            _customerServices = customerServices;
        }


        // POST: api/Customers
        [HttpPost]
        public async Task<ActionResult<Customer>> CreateCustomer(Customer customer)
        {
            await _customerServices.CreateNewCustomerAsync(customer);
            _customerServices.SendCustomerCreatedEventAsync(customer);

            return CreatedAtAction(nameof(CreateCustomer), new { id = customer.Id }, customer);
        }


        // GET: api/customers
        [HttpGet]
        public async Task<IActionResult> GetAllCustomers() 
        {
            try
            {
                var customers = await _customerServices.GetAllCustomersAsync();
                
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<IActionResult> SearchCustomerById(int id) 
        {
            try
            {
                var customer = await _customerServices.SearchCustomerByIdAsync(id);
                
                return Ok(customer);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }



        
    }
}
