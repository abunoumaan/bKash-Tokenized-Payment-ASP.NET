using System;
using System.Data.Entity;
using bKashPayment.Models;

namespace bKashPayment
{
    /// <summary>
    /// Entity Framework Database Context
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("name=DefaultConnection")
        {
            // Enable automatic migrations
        }

        // DbSets
        public DbSet<Customer> Customers { get; set; }
        public DbSet<BkashAgreement> BkashAgreements { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure one-to-many relationships
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Agreements)
                .WithRequired(a => a.Customer)
                .HasForeignKey(a => a.CustomerID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Transactions)
                .WithRequired(t => t.Customer)
                .HasForeignKey(t => t.CustomerID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<BkashAgreement>()
                .HasMany(a => a.Transactions)
                .WithRequired(t => t.Agreement)
                .HasForeignKey(t => t.AgreementID)
                .WillCascadeOnDelete(false);
        }
    }
}
