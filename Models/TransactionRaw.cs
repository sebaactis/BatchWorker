namespace BatchProcessing.Models
{
    public class TransactionRaw
    {
        public long TransactionId { get; set; }
        public string MerchantId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime Date { get; set; }
    }
}
