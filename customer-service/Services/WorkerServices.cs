using System.Text;
using System.Text.Json;
using CustomerService.Data;
using CustomerService.Models;
using CustomerService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace CustomerService.Services
{
    public class WorkerServices : IWorkerServices
    {
        private readonly IServiceProvider _serviceProvider;

        
        public WorkerServices(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<ProcessStatus> UpdateCreditProposal(CreditProposal creditProposalObject) 
        {
            try 
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<BankContext>();

                    Customer? customer = await dbContext.Customers.FindAsync(creditProposalObject.CustomerId);

                    if (customer == null)
                    {
                        throw new Exception($"Client with ID {creditProposalObject.CustomerId} not found.");
                    }

                    CreditProposal? existingCreditProposal = await dbContext.CreditProposals
                        .FirstOrDefaultAsync(cp => cp.CustomerId == creditProposalObject.CustomerId);

                    customer.AvailableCredit = creditProposalObject.AvailableCredit;

                    if (existingCreditProposal != null)
                    {
                        existingCreditProposal.AvailableCredit = creditProposalObject.AvailableCredit;
                        dbContext.CreditProposals.Update(existingCreditProposal);
                        
                        dbContext.Customers.Update(customer);

                    }
                    else
                    {
                        dbContext.CreditProposals.Add(creditProposalObject);
                        dbContext.Customers.Update(customer);
                    }

                    await dbContext.SaveChangesAsync();
                    Console.WriteLine("Credit Proposal saved successfully.");

                    return ProcessStatus.Success;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save the Credit Proposal in the database. Error: {ex.Message}");
                //_channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                return ProcessStatus.UnexpectedError;
            }
        }



        public async Task<ProcessStatus> ProcessCreditCardProposalQueue(string message) 
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            try
            {
                var creditCardProposalObject = JsonSerializer.Deserialize<CreditCardProposal>(message, options);

                if (creditCardProposalObject != null)
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<BankContext>();

                        var existingCustomer = await dbContext.Customers.FindAsync(creditCardProposalObject.CustomerId);

                        if (existingCustomer != null) 
                        {
                            existingCustomer.CreditCards = creditCardProposalObject.CreditCards;
                            await dbContext.SaveChangesAsync();

                            Console.WriteLine("Customer depois de modificar:");
                            Console.WriteLine($"Id: {existingCustomer.Id}, Name: {existingCustomer.Name}, Email: {existingCustomer.Email}, Score: {existingCustomer.Score}");
                            if (existingCustomer.CreditCards != null)
                            {
                                foreach (var card in existingCustomer.CreditCards)
                                {
                                    Console.WriteLine($"CreditCard Id: {card.Id}, CustomerId: {card.CustomerId}, CardNumber: {card.CardNumber}, ExpiringDate: {card.CardExpiringDate}");
                                }
                            }

                            return ProcessStatus.Success;
                        }
                        else
                        {
                            Console.WriteLine($"ERROR: Customer with Id {creditCardProposalObject.CustomerId} doesn't exists in database.");
                            return ProcessStatus.NotFound;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Failed to deserialize the message to a Customer object.");
                    return ProcessStatus.DeserializationError;
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing JSON: {ex.Message}");
                return ProcessStatus.DeserializationError;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return ProcessStatus.UnexpectedError;
            }
        }

    }
}

