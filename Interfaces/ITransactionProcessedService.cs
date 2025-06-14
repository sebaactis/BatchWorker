namespace BatchProcessing.Interfaces
{
    internal interface ITransactionProcessedService<T>
    {
        Task<bool> ProcessesTransactions();
        Task<bool> CreateOUTFile();
    }
}
