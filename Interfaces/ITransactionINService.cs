namespace BatchProcessing.Interfaces
{
    public interface ITransactionINService<T>
    {
        Task<int> Save(List<T> entity, CancellationToken? ct);
    }
}
