namespace CustomerService.Models
{
    public class CreditCard
    {
        public required int Id { get; set; }
        public required int CustomerId { get; set; }
        public required string? CardNumber { get; set; }
        public required DateTime CardExpiringDate { get; set; }
    }
}