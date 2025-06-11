namespace BatchProcessing.Interfaces
{
    internal interface ITransactionProcessedService<T>
    {
        Task ProcessesTransactions();
    }
}
