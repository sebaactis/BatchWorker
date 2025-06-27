namespace BatchProcessing.Interfaces.Utils
{
    public interface IFileReader<T> where T : class
    {
        IEnumerable<T> Read();
    }
}
