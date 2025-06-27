namespace BatchProcessing.Interfaces.Services
{
    internal interface ITransactionProcessedService<T>
    {
        Task<(int successInserts, int failedPermanentlyInserts, int failedValidationInserts)> ProcessesTransactions();
        Task<(int successProcessed, int failedProcessed)> ReprocessedFailedPermanentlyTransactions();
        Task<int> CreateOUTFile();
    }
}
