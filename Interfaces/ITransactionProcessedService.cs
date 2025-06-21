namespace BatchProcessing.Interfaces
{
    internal interface ITransactionProcessedService<T>
    {
        Task<(int successInserts, int failedPermanentlyInserts, int failedValidationInserts)> ProcessesTransactions();
        Task<int> CreateOUTFile();
    }
}
