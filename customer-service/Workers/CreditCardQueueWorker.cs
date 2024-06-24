using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CustomerService.Data;
using CustomerService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using static System.Formats.Asn1.AsnWriter;

namespace CustomerService.Workers
{
    public class CreditCardQueueWorker : BackgroundService
    {
        // private readonly CustomerContext _context;
        private readonly ConnectionFactory _factory;
        private IModel _channel;
        private readonly IConnection _connection;
        private readonly IServiceProvider _serviceProvider;
        public List<Customer> Customers { get; private set; }


        public CreditCardQueueWorker(ConnectionFactory factory, IServiceProvider serviceProvider)
        {
            _factory = factory;
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            _serviceProvider = serviceProvider;

            _channel.ExchangeDeclare(exchange: "customer_exchange",
                         type: ExchangeType.Direct,
                         durable: true,
                         autoDelete: false,
                         arguments: null);

            _channel.QueueBind(queue: "credit_card_queue",
                            exchange: "customer_exchange",
                            routingKey: "new_customer");


            Customers =  new List<Customer>();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            Console.WriteLine("RabbitMQWorker is starting...");

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
                    var customer = JsonSerializer.Deserialize<Customer>(message, options);

                    if (customer != null)
                    {
                        Console.WriteLine($"Customer received: Id={customer.Id}, Name={customer.Name}, Email={customer.Email}, Score={customer.Score}");

                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<CustomerContext>();

                            var existingCustomer = await dbContext.Customers.FindAsync(customer.Id);
                            
                            if (existingCustomer == null)
                            {
                                dbContext.Customers.Add(customer);
                                var result = await dbContext.SaveChangesAsync();

                                if (result > 0)
                                {
                                    Console.WriteLine("Customer saved successfully.");
                                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                                }
                                else
                                {
                                    Console.WriteLine("Failed to save customer.");
                                    //_channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Customer with Id {customer.Id} already exists. Skipping.");
                                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
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

            _channel.BasicConsume(queue: "new_customer_queue", autoAck: false, consumer: consumer);

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
