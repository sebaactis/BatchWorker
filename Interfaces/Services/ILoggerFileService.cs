using BatchProcessing.Enums;
using BatchProcessing.Models.DTO;

namespace BatchProcessing.Interfaces.Services
{
    public interface ILoggerFileService
    {
        void Log(string message, LogLevelCustom levelLog, string? subPath);
        void LogTotalProcess(DailyResumeDTO processResumen, LogLevelCustom levelLog);
    }
}
