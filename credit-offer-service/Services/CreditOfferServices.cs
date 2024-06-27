using System.Text;
using System.Text.Json;
using CreditOfferService.Models;
using CreditOfferService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace CreditOfferService.Services
{
    public class CreditOfferServices : ICreditOfferServices
    {
        private readonly ConnectionFactory _factory;

        
        public CreditOfferServices(ConnectionFactory factory)
        {
            _factory = factory;
        }
        
        public void SendCreditOfferEventAsync(Customer customer)
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();

            var newCreditProposal = new CreditProposal
            {
            CustomerId = customer.Id,
            AvailableCredit = customer.Score > 900 ? 10000 :
                               customer.Score > 500 ? 5000 :
                               customer.Score > 300 ? 1000 :
                               0
            };

            var message = System.Text.Json.JsonSerializer.Serialize(newCreditProposal);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "customer_exchange",
                                 routingKey: "credit_proposal",
                                 basicProperties: null,
                                 body: body);
        }

    }
}

