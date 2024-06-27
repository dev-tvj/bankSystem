using System.Text;
using System.Text.Json;
using CreditOfferService.Models;
using CreditOfferService.Services.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CreditOfferService.Workers
{
    public class CreditOfferQueueWorker : BackgroundService
    {
        private readonly ConnectionFactory _factory;
        private IModel _channel;
        private readonly IConnection _connection;
        private readonly IServiceProvider _serviceProvider;
        private readonly ICreditOfferServices _creditOfferServices;


        public CreditOfferQueueWorker(ConnectionFactory factory, IServiceProvider serviceProvider, ICreditOfferServices creditOfferServices)
        {
            _factory = factory;
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            _serviceProvider = serviceProvider;
            _creditOfferServices = creditOfferServices;

            _channel.ExchangeDeclare(exchange: "customer_exchange",
                                    type: ExchangeType.Direct,
                                    durable: true,
                                    autoDelete: false,
                                    arguments: null);

            _channel.QueueBind(queue: "new_customer_queue",
                                exchange: "customer_exchange",
                                routingKey: "new_customer");

        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            Console.WriteLine("RabbitMQ CreditOfferQueueWorker is starting...");

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, ea) =>
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

                        _creditOfferServices.SendCreditOfferEventAsync(customer);
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
