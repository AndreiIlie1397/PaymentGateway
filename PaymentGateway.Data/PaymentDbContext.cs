using Microsoft.EntityFrameworkCore;
using PaymentGateway.Data.EntityTypeConfiguration;
using PaymentGateway.Models;

namespace PaymentGateway.Data
{
    public class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
        {

        }

        public DbSet<Person> Persons { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<ProductXTransaction> ProductXTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().HasKey(x => x.Id);
            modelBuilder.Entity<Product>().Property(x => x.Id);//.HasColumnName("IdUlMeuSpecial");

            modelBuilder.Entity<ProductXTransaction>().HasKey(x => new { x.ProductId, x.TransactionId });

            modelBuilder.ApplyConfiguration(new PersonConfiguration());
        }
    }
}
