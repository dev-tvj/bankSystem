using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CreditOfferService.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        
        [Required(ErrorMessage = "Field is required.")]
        public string Name { get; set; }

        
        [Required(ErrorMessage = "Field is required.")]
        [EmailAddress(ErrorMessage = "Invalid format.")]
        public string Email { get; set; }

        
        [Required(ErrorMessage = "Field is required.")]
        [RegularExpression(@"^\d{3}\.\d{3}\.\d{3}\-\d{2}$", ErrorMessage = "Invalid format.")]
        public string Cpf { get; set; }

        [Required(ErrorMessage = "Field is required.")]
        [Range(0, 1000, ErrorMessage = "Out of valid range.")]
        public int Score { get; set; }

        
        [Required(ErrorMessage = "Field is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Out of valid range.")]
        public decimal Balance { get; set; }

        
        [Range(0, double.MaxValue, ErrorMessage = "Out of valid range.")]
        public decimal AvailableCredit { get; set; }
        
        public List<CreditCard> CreditCards { get; set; }
        public List<CreditProposal> CreditProposals { get; set; }
        public List<CreditCardProposal> CreditCardProposals { get; set; }
    }
}