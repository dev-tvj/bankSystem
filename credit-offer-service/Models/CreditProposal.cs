using System.ComponentModel.DataAnnotations;

namespace CreditOfferService.Models
{
    public class CreditProposal
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int CustomerId { get; set; }
        
        [Required]
        public decimal AvailableCredit { get; set;}
    }
}