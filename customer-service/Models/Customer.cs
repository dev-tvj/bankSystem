namespace CustomerService.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public required String Name { get; set; }
        public required String Email { get; set; }
        public required int Cpf { get; set; }
        public int Score { get; set;}
        public required Boolean ScoreStatus { get; set;}
        public required decimal Balance { get; set;}
        public List<CreditCard>? CreditCards {get; set;}
        public decimal AvailableCredit {get; set;}
    }
}