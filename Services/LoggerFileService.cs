using BatchProcessing.Enums;
using BatchProcessing.Interfaces;

namespace BatchProcessing.Services
{
    public class LoggerFileService : ILoggerFileService
    {
        private readonly ILogger<Worker> _logger;

        public LoggerFileService(ILogger<Worker> logger)
        {
            _logger = logger;
        }
        public void Log(string message, LogLevelCustom levelLog, string? subPath)
        {
            string actualDate = DateTime.Now.ToString("dd-MM-yyyy:hh:mm:ss");
            string baseLogDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            string logDirectory = string.IsNullOrWhiteSpace(subPath)
                ? baseLogDirectory
                : Path.Combine(baseLogDirectory, subPath);

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            string fileName = $"log_{DateTime.Now.ToString("dd-MM-yyyy")}.txt";
            string filePath = Path.Combine(logDirectory, fileName);


            string messageToSave = $"Fecha: {actualDate} /// Nivel:  {levelLog.ToString().ToUpper()} /// Mensaje: {message} {Environment.NewLine}";
            LogMessage(message, levelLog);

            File.AppendAllText(filePath, messageToSave);
        }

        public void LogMessage(string message, LogLevelCustom levelLog)
        {
            switch (levelLog)
            {
                case LogLevelCustom.Info:
                    _logger.LogInformation(message);
                    break;
                case LogLevelCustom.Error:
                    _logger.LogError(message);
                    break;
                case LogLevelCustom.Warning:
                    _logger.LogWarning(message);
                    break;
                case LogLevelCustom.Debug:
                    _logger.LogDebug(message);
                    break;
                case LogLevelCustom.Critical:
                    _logger.LogCritical(message);
                    break;
                default:
                    _logger.LogInformation(message);
                    break;
            }
        }
    }
}
