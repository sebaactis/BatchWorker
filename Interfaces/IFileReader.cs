namespace BatchProcessing.Interfaces
{
    public interface IFileReader<T> where T : class
    {
        IEnumerable<T> Read();
    }
}
