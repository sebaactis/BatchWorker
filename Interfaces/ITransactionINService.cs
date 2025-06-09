namespace BatchProcessing.Interfaces
{
    public interface ITransactionINService<T>
    {
        Task<T> Save(T entity);
    }
}
