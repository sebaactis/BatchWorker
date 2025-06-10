namespace BatchProcessing.Interfaces
{
    public interface ITransactionINService<T>
    {
        Task Save(List<T> entity, CancellationToken? ct);
    }
}
