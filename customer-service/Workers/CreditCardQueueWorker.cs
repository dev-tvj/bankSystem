using System.Text;

using CustomerService.Services;
using CustomerService.Services.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CustomerService.Workers
{
    public class CreditCardQueueWorker : BackgroundService
    {
        private readonly IWorkerServices _workerServices;
        private readonly ConnectionFactory _factory;
        private readonly IModel _channel;
        private readonly IConnection _connection;
        private readonly IServiceProvider _serviceProvider;


        public CreditCardQueueWorker(ConnectionFactory factory, IServiceProvider serviceProvider, IWorkerServices workerServices)
        {
            _workerServices = workerServices;

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
                            routingKey: "credit_card");
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            Console.WriteLine("RabbitMQ CreditCardQueueWorker is starting...");

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine($"CreditCardQueueWorker Received message: {message}");

                await _workerServices.ProcessCreditCardProposalQueue(message);

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                
            };

            _channel.BasicConsume(queue: "credit_card_queue", autoAck: false, consumer: consumer);

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
