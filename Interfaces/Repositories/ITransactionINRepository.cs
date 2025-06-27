namespace BatchProcessing.Interfaces.Repositories
{
    public interface ITransactionINRepository<T>
    {
        void AddRange(IEnumerable<T> entities);
        IEnumerable<T> FindToProcess();
        IEnumerable<T> FindToReprocessPermanently();
        Task SaveChangesAsync();
    }
}
