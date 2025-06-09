namespace BatchProcessing.Interfaces
{
    public interface ITransactionINRepository<T>
    {
        Task<T> Save(T entity);
    }
}
