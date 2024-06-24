namespace CustomerService.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }

        public required int ScoreNumber { get; set;}

        public Boolean ScoreStatus { get; set;}
    }
}
