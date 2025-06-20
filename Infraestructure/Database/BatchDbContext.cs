using BatchProcessing.Models;
using Microsoft.EntityFrameworkCore;

namespace BatchProcessing.Infraestructure.Database
{
    public class BatchDbContext : DbContext
    {
        public DbSet<TransactionRaw> Transactions_IN { get; set; }
        public DbSet<TransactionProcessed> Transactions_PROCESSED { get; set; }
        public BatchDbContext(DbContextOptions<BatchDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TransactionRaw>().ToTable("TRANSACTIONS_IN");
            modelBuilder.Entity<TransactionProcessed>().ToTable("TRANSACTIONS_PROCESSED");

            modelBuilder.Entity<TransactionRaw>(entity =>
            {
                entity.HasKey(e => e.TransactionId);
                entity.Property(e => e.TransactionId)
                .HasColumnName("TransactionId")
                .ValueGeneratedOnAdd();

                entity.Property(e => e.MerchantId)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnName("MerchantId");

                entity.Property(e => e.MerchantName)
                    .HasMaxLength(100)
                    .HasColumnName("MerchantName");

                entity.Property(e => e.MerchantCountry)
                    .HasMaxLength(3)
                    .HasColumnName("MerchantCountry");

                entity.Property(e => e.MerchantCategoryCode)
                    .HasMaxLength(4)
                    .HasColumnName("MerchantCategoryCode");

                entity.Property(e => e.CardHolderName)
                    .HasMaxLength(100)
                    .HasColumnName("CardHolderName");

                entity.Property(e => e.CardNumberMasked)
                    .HasMaxLength(19)
                    .HasColumnName("CardNumberMasked");

                entity.Property(e => e.CardType)
                    .HasMaxLength(20)
                    .HasColumnName("CardType");

                entity.Property(e => e.CustomerId)
                    .HasMaxLength(50)
                    .HasColumnName("CustomerId");

                entity.Property(e => e.Amount)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)")
                    .HasColumnName("Amount");

                entity.Property(e => e.AmountLocalCurrency)
                    .HasColumnType("decimal(18,2)")
                    .HasColumnName("AmountLocalCurrency");

                entity.Property(e => e.Currency)
                    .IsRequired()
                    .HasMaxLength(3)
                    .HasColumnName("Currency");

                entity.Property(e => e.LocalCurrency)
                    .HasMaxLength(3)
                    .HasColumnName("LocalCurrency");

                entity.Property(e => e.ExchangeRate)
                    .HasColumnType("decimal(18,6)")
                    .HasColumnName("ExchangeRate");

                entity.Property(e => e.Date)
                    .HasColumnType("timestamp without time zone")
                    .IsRequired()
                    .HasColumnName("Date");

                entity.Property(e => e.PostingDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("PostingDate");

                entity.Property(e => e.AuthorizationDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("AuthorizationDate");

                entity.Property(e => e.TerminalId)
                    .HasMaxLength(20)
                    .HasColumnName("TerminalId");

                entity.Property(e => e.POSLocation)
                    .HasMaxLength(100)
                    .HasColumnName("POSLocation");

                entity.Property(e => e.POSCountryCode)
                    .HasMaxLength(3)
                    .HasColumnName("POSCountryCode");

                entity.Property(e => e.EntryMode)
                    .HasMaxLength(20)
                    .HasColumnName("EntryMode");

                entity.Property(e => e.AuthorizationCode)
                    .HasMaxLength(20)
                    .HasColumnName("AuthorizationCode");

                entity.Property(e => e.TransactionType)
                    .HasMaxLength(20)
                    .HasColumnName("TransactionType");

                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .HasColumnName("Status");

                entity.Property(e => e.IsInternational)
                    .HasColumnName("IsInternational");

                entity.Property(e => e.IsFraudSuspected)
                    .HasColumnName("IsFraudSuspected");

                entity.Property(e => e.IsOfflineTransaction)
                    .HasColumnName("IsOfflineTransaction");

                entity.Property(e => e.IsProcessed)
                    .HasColumnName("IsProcessed");

                entity.Property(e => e.Notes)
                    .HasColumnType("text")
                    .HasColumnName("Notes");

                entity.Property(e => e.ReferenceNumber)
                    .HasMaxLength(50)
                    .HasColumnName("ReferenceNumber");

                entity.Property(e => e.RetryCount)
                    .HasDefaultValue(0)
                    .HasColumnName("RetryCount");

                entity.Property(e => e.FailedPermanently)
                    .HasColumnName("FailedPermanently");
            });

            modelBuilder.Entity<TransactionProcessed>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                      .HasColumnName("Id")
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.TransactionId)
                      .HasColumnName("TransactionId");

                entity.Property(e => e.MerchantId)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnName("MerchantId");

                entity.Property(e => e.MerchantName)
                    .HasMaxLength(100)
                    .HasColumnName("MerchantName");

                entity.Property(e => e.MerchantCountry)
                    .HasMaxLength(3)
                    .HasColumnName("MerchantCountry");

                entity.Property(e => e.MerchantCategoryCode)
                    .HasMaxLength(4)
                    .HasColumnName("MerchantCategoryCode");

                entity.Property(e => e.CardHolderName)
                    .HasMaxLength(100)
                    .HasColumnName("CardHolderName");

                entity.Property(e => e.CardNumberMasked)
                    .HasMaxLength(19)
                    .HasColumnName("CardNumberMasked");

                entity.Property(e => e.CardType)
                    .HasMaxLength(20)
                    .HasColumnName("CardType");

                entity.Property(e => e.CustomerId)
                    .HasMaxLength(50)
                    .HasColumnName("CustomerId");

                entity.Property(e => e.Amount)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)")
                    .HasColumnName("Amount");

                entity.Property(e => e.AmountLocalCurrency)
                    .HasColumnType("decimal(18,2)")
                    .HasColumnName("AmountLocalCurrency");

                entity.Property(e => e.Currency)
                    .IsRequired()
                    .HasMaxLength(3)
                    .HasColumnName("Currency");

                entity.Property(e => e.LocalCurrency)
                    .HasMaxLength(3)
                    .HasColumnName("LocalCurrency");

                entity.Property(e => e.ExchangeRate)
                    .HasColumnType("decimal(18,6)")
                    .HasColumnName("ExchangeRate");

                entity.Property(e => e.Date)
                    .HasColumnType("timestamp without time zone")
                    .IsRequired()
                    .HasColumnName("Date");

                entity.Property(e => e.PostingDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("PostingDate");

                entity.Property(e => e.AuthorizationDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("AuthorizationDate");

                entity.Property(e => e.ConciliatedDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("ConciliatedDate");

                entity.Property(e => e.TerminalId)
                    .HasMaxLength(20)
                    .HasColumnName("TerminalId");

                entity.Property(e => e.POSLocation)
                    .HasMaxLength(100)
                    .HasColumnName("POSLocation");

                entity.Property(e => e.POSCountryCode)
                    .HasMaxLength(3)
                    .HasColumnName("POSCountryCode");

                entity.Property(e => e.EntryMode)
                    .HasMaxLength(20)
                    .HasColumnName("EntryMode");

                entity.Property(e => e.AuthorizationCode)
                    .HasMaxLength(20)
                    .HasColumnName("AuthorizationCode");

                entity.Property(e => e.TransactionType)
                    .HasMaxLength(20)
                    .HasColumnName("TransactionType");

                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .HasColumnName("Status");

                entity.Property(e => e.IsInternational)
                    .HasColumnName("IsInternational");

                entity.Property(e => e.IsFraudSuspected)
                    .HasColumnName("IsFraudSuspected");

                entity.Property(e => e.IsOfflineTransaction)
                    .HasColumnName("IsOfflineTransaction");

                entity.Property(e => e.IsConciliated)
                    .HasColumnName("IsConciliated");

                entity.Property(e => e.Notes)
                    .HasColumnType("text")
                    .HasColumnName("Notes");

                entity.Property(e => e.ReferenceNumber)
                    .HasMaxLength(50)
                    .HasColumnName("ReferenceNumber");

                entity.Property(e => e.ProcessedDate)
                    .HasColumnName("ProcessedDate");
            });
        }
    }
}
