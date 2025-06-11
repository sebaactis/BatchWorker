namespace BatchProcessing.Models
{
    public class TransactionRaw
    {
        public long TransactionId { get; set; }

        // Datos del comercio
        public string MerchantId { get; set; }
        public string MerchantName { get; set; }
        public string MerchantCountry { get; set; }
        public string MerchantCategoryCode { get; set; }

        // Datos del cliente
        public string CardHolderName { get; set; }
        public string CardNumberMasked { get; set; }
        public string CardType { get; set; } // Visa, Mastercard, etc.
        public string CustomerId { get; set; }

        // Monto y moneda
        public decimal Amount { get; set; }
        public decimal AmountLocalCurrency { get; set; }
        public string Currency { get; set; }
        public string LocalCurrency { get; set; }
        public decimal ExchangeRate { get; set; }

        // Fechas
        public DateTime Date { get; set; }
        public DateTime PostingDate { get; set; }
        public DateTime AuthorizationDate { get; set; }

        // Geolocalización
        public string TerminalId { get; set; }
        public string POSLocation { get; set; }
        public string POSCountryCode { get; set; }

        // Información técnica
        public string EntryMode { get; set; } // Chip, Swipe, Contactless
        public string AuthorizationCode { get; set; }
        public string TransactionType { get; set; } // Purchase, Refund, Reversal, etc.
        public string Status { get; set; } // Pending, Approved, Declined, Reversed

        // Flags adicionales
        public bool IsInternational { get; set; }
        public bool IsFraudSuspected { get; set; }
        public bool IsOfflineTransaction { get; set; }
        public bool IsProcessed { get; set; } = false;

        // Otros
        public string Notes { get; set; }
        public string ReferenceNumber { get; set; }
    }
}