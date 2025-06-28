namespace BatchProcessing.Interfaces.Services
{
    public interface ITransactionProcessedService<T>
    {
        Task<(int successInserts, int failedPermanentlyInserts, int failedValidationInserts)> ProcessesTransactions();
        Task<(int successProcessed, int failedProcessed)> ReprocessedFailedPermanentlyTransactions();
        Task<int> CreateOUTFile();
    }
}
