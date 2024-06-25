using Microsoft.EntityFrameworkCore;
using CustomerService.Models;

namespace CustomerService.Data
{
    public class BankContext : DbContext
    {
        public BankContext(DbContextOptions<BankContext> options) : base(options) { }

        public DbSet<CreditCard> CreditCards { get; set; }
        public DbSet<CreditCardProposal>? CreditCardProposals { get; set; }
        public DbSet<CreditProposal> CreditProposals { get; set; }
        public DbSet<Customer> Customers { get; set; }
    }
}