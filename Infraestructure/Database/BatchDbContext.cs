using BatchProcessing.Models;
using Microsoft.EntityFrameworkCore;

namespace BatchProcessing.Infraestructure.Database
{
    public class BatchDbContext : DbContext
    {

        // TO DO:
        // 1 - Arreglar para poder grabar hora en este formato -> DD/MM/YYYY HH:MM:SS
        // 2 - Validar antes de grabar en la base de datos
        // 3 - Crear una transaccion de base para poder hacer rollback y no grabar en caso de errores.
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
