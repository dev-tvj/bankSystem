using CreditOfferService.Models;

namespace CreditOfferService.Services.Interfaces
{
    public interface ICreditOfferServices
    {
        void SendCreditOfferEventAsync(Customer customer);
    }
}