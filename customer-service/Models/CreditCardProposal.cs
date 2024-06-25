namespace CustomerService.Models
{
    public class CreditCardProposal
    {
        public required int Id { get; set; }
        public required int CustomerId { get; set; }
        public required List<CreditCard> CreditCards { get; set; }
    }
}