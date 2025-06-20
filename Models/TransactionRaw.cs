using CsvHelper.Configuration.Attributes;

namespace BatchProcessing.Models
{
    public class TransactionRaw
    {
        [Ignore]
        public long TransactionId { get; set; }
        public string MerchantId { get; set; }
        public string MerchantName { get; set; }
        public string MerchantCountry { get; set; }
        public string MerchantCategoryCode { get; set; }

        public string CardHolderName { get; set; }
        public string CardNumberMasked { get; set; }
        public string CardType { get; set; } // Visa, Mastercard, etc.
        public string CustomerId { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountLocalCurrency { get; set; }
        public string Currency { get; set; }
        public string LocalCurrency { get; set; }
        public decimal ExchangeRate { get; set; }
        public DateTime Date { get; set; }
        public DateTime PostingDate { get; set; }
        public DateTime AuthorizationDate { get; set; }
        public string TerminalId { get; set; }
        public string POSLocation { get; set; }
        public string POSCountryCode { get; set; }
        public string EntryMode { get; set; } // Chip, Swipe, Contactless
        public string AuthorizationCode { get; set; }
        public string TransactionType { get; set; } // Purchase, Refund, Reversal, etc.
        public string Status { get; set; } // Pending, Approved, Declined, Reversed
        public bool IsInternational { get; set; }
        public bool IsFraudSuspected { get; set; }
        public bool IsOfflineTransaction { get; set; }
        public bool IsProcessed { get; set; } = false;
        public string Notes { get; set; }
        public string ReferenceNumber { get; set; }

        [Ignore]
        public int RetryCount { get; set; } = 0;

        [Ignore]
        public bool FailedPermanently { get; set; } = false;
    }
}