namespace BatchProcessing.Models.DTO
{
    public class DailyResumeDTO
    {
        public long TotalTransactions { get; set; }
        public long SuccessfulProcessedTransactionsIN { get; set; }
        public long SuccessfulProcessedTransactionsProcessed { get; set; }
        public long FailedValidationTransactionsIN { get; set; }
        public long FailedValidationTransactionsProcessed { get; set; }
        public long PermanentFailedTransactions { get; set; }
        public long TotalOUTTransactions { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public string TotalTimeProcess { get; set; } = string.Empty;
    }
}
