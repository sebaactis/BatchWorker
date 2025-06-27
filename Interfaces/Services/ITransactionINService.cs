namespace BatchProcessing.Interfaces.Services
{
    public interface ITransactionINService<T>
    {
        Task<int> Save(List<T> entity, CancellationToken? ct);
    }
}
