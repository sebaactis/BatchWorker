using BatchProcessing.Enums;

namespace BatchProcessing.Interfaces
{
    public interface ILoggerFileService
    {
        void Log(string message, LogLevelCustom levelLog, string? subPath);
    }
}
