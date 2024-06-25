namespace CustomerService.Models
{
    public class CreditProposal
    {
        public required int Id { get; set; }
        public required int CustomerId { get; set; }
        public required decimal AvailableCredit { get; set;}
    }
}