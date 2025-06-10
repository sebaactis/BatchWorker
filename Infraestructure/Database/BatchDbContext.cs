using BatchProcessing.Models;
using Microsoft.EntityFrameworkCore;

namespace BatchProcessing.Infraestructure.Database
{
    public class BatchDbContext : DbContext
    {
        public DbSet<TransactionRaw> Transactions_IN { get; set; }
        public BatchDbContext(DbContextOptions<BatchDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TransactionRaw>().ToTable("TRANSACTIONS_IN");

            modelBuilder.Entity<TransactionRaw>(entity =>
            {
                entity.HasKey(e => e.TransactionId);
                entity.Property(e => e.MerchantId).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Amount).IsRequired();
                entity.Property(e => e.Currency).IsRequired().HasMaxLength(3);
                entity.Property(e => e.Date).HasColumnType("timestamp without time zone").IsRequired();
            });
        }
    }
}
