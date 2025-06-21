using BatchProcessing.Enums;
using BatchProcessing.Models.DTO;

namespace BatchProcessing.Interfaces
{
    public interface ILoggerFileService
    {
        void Log(string message, LogLevelCustom levelLog, string? subPath);
        void LogTotalProcess(DailyResumeDTO processResumen, LogLevelCustom levelLog);
    }
}
