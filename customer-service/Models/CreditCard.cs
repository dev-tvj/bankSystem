using System;
using System.ComponentModel.DataAnnotations;

namespace CustomerService.Models
{
    public class CreditCard
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int CreditCardProposalId { get; set; }

        [Required]
        public string CardNumber { get; set; }

        [Required]
        public DateTime CardExpiringDate { get; set; }
    }

}
