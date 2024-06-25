namespace CustomerService.Models
{
    public class CreditCard
    {
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CardNumber { get; set; }
    public DateTime CardExpiringDate { get; set; }    
    }
}