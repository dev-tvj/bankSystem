using CustomerService.Models;

namespace CustomerService.Services.Interfaces
{
    public interface IWorkerServices
    {
        Task<ProcessStatus> ProcessCreditCardProposalQueue(string message);
        Task<ProcessStatus> UpdateCreditProposal(CreditProposal creditProposal);

    }
}










