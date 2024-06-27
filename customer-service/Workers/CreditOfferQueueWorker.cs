using System.Text;
using System.Text.Json;
using CustomerService.Data;
using CustomerService.Models;
using CustomerService.Services.Interfaces;
using Microsoft.VisualBasic;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CustomerService.Workers
{
    public class CreditOfferQueueWorker : BackgroundService
    {
        // private readonly BankContext _context;
        private readonly ConnectionFactory _factory;
        private IModel _channel;
        private readonly IConnection _connection;
        private readonly IServiceProvider _serviceProvider;
        private readonly IWorkerServices _workerServices;



        public CreditOfferQueueWorker(ConnectionFactory factory, IServiceProvider serviceProvider, IWorkerServices workerServices)
        {
            _factory = factory;
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            _serviceProvider = serviceProvider;
            _workerServices = workerServices;

            _channel.ExchangeDeclare(exchange: "customer_exchange",
                                    type: ExchangeType.Direct,
                                    durable: true,
                                    autoDelete: false,
                                    arguments: null);

            _channel.QueueBind(queue: "credit_proposal_queue",
                                exchange: "customer_exchange",
                                routingKey: "credit_proposal");



        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            Console.WriteLine("RabbitMQ CreditOfferQueueWorker is starting...");

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine($"Received message: {message}");

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                try
                {
                    var creditProposalObject = JsonSerializer.Deserialize<CreditProposal>(message, options);

                    if (creditProposalObject != null)
                    {
                        Console.WriteLine($"Customer received: Id={creditProposalObject.Id}, CustomerId={creditProposalObject.CustomerId}, AvailableCredit={creditProposalObject.AvailableCredit}");

                        using (var scope = _serviceProvider.CreateScope())
                        {
                            try 
                            {
                                var dbContext = scope.ServiceProvider.GetRequiredService<BankContext>();

                                Customer customer = await dbContext.Customers.FindAsync(creditProposalObject.CustomerId) ?? throw new Exception($"Client with ID {creditProposalObject.CustomerId} not found.");

                                await _workerServices.UpdateCreditProposal(creditProposalObject);
                                //dbContext.CreditProposals.Add(creditProposalObject);
                                //dbContext.Customers.Add(customer);

                                await dbContext.SaveChangesAsync();
                                Console.WriteLine("Credit Proposal saved successfully.");
                            }
                            catch
                            {
                                Console.WriteLine("Failed to save the Credit Proposal in the database.");
                                //_channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Failed to deserialize the message to a Customer object.");
                        //_channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                    }
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Error deserializing JSON: {ex.Message}");
                    //_channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                    //_channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            _channel.BasicConsume(queue: "credit_proposal_queue", autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }


        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
