namespace BatchProcessing.Interfaces.Repositories
{
    public interface ITransactionProcessesRepository<T>
    {
        Task Save(IEnumerable<T> transactions);
        Task SaveChanges();
        IEnumerable<T> FindToProcess();
    }
}
