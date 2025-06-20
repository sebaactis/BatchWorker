namespace BatchProcessing.Interfaces
{
    public interface ITransactionINRepository<T>
    {
        void AddRange(IEnumerable<T> entities);
        IEnumerable<T> FindToProcess();
        Task SaveChangesAsync();
    }
}
