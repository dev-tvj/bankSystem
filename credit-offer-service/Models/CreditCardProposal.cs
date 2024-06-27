using System.ComponentModel.DataAnnotations;

namespace CreditOfferService.Models
{
    public class CreditCardProposal
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public List<CreditCard> CreditCards { get; set; }
    }
}