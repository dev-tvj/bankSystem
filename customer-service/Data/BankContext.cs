using Microsoft.EntityFrameworkCore;
using CustomerService.Models;

namespace CustomerService.Data
{
    public class BankContext : DbContext
    {
        public BankContext(DbContextOptions<BankContext> options) : base(options) { }

        public DbSet<CreditCard> CreditCards { get; set; }
        public DbSet<CreditCardProposal> CreditCardProposals { get; set; }
        public DbSet<CreditProposal> CreditProposals { get; set; }
        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CreditCard>()
                .HasOne<Customer>()
                .WithMany(c => c.CreditCards)
                .HasForeignKey(cc => cc.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CreditCard>()
                .HasOne<CreditCardProposal>()
                .WithMany(c => c.CreditCards)
                .HasForeignKey(cc => cc.CreditCardProposalId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CreditProposal>()
                .HasOne<Customer>()
                .WithMany(c => c.CreditProposals)
                .HasForeignKey(cc => cc.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CreditCardProposal>()
                .HasOne<Customer>()
                .WithMany(c => c.CreditCardProposals)
                .HasForeignKey(cc => cc.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }


    }
}
