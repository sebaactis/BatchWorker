using BatchProcessing.Enums;
using BatchProcessing.Interfaces.Services;
using BatchProcessing.Models.DTO;

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

        public void LogTotalProcess(DailyResumeDTO processResumen, LogLevelCustom levelLog)
        {
            string logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs/Resume");

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            string fileName = $"Resume_Process_{DateTime.Now.ToString("dd-MM-yyyy")}.txt";
            string filePath = Path.Combine(logDirectory, fileName);

            string messageToSave =
                                    $"{Environment.NewLine}" +
                                    $"=========== Fecha de ejecución: {processResumen.StartTime:dd-MM-yyyy:HH:mm:ss} ==========={Environment.NewLine}" +
                                    $"/// {"Total Transacciones leídas:".PadRight(45)} {processResumen.TotalTransactions}{Environment.NewLine}" +
                                    $"/// {"Transacciones Exitosas IN:".PadRight(45)} {processResumen.SuccessfulProcessedTransactionsIN}{Environment.NewLine}" +
                                    $"/// {"Transacciones Fallidas Validación IN:".PadRight(45)} {processResumen.FailedValidationTransactionsIN}{Environment.NewLine}" +
                                    $"/// {"Transacciones Exitosas Processed:".PadRight(45)} {processResumen.SuccessfulProcessedTransactionsProcessed}{Environment.NewLine}" +
                                    $"/// {"Transacciones Fallidas Validación Processed:".PadRight(45)} {processResumen.FailedValidationTransactionsProcessed}{Environment.NewLine}" +
                                    $"/// {"Transacciones Fallidas Permanentes:".PadRight(45)} {processResumen.PermanentFailedTransactions}{Environment.NewLine}" +
                                    $"/// {"Total OUT Transacciones:".PadRight(45)} {processResumen.TotalOUTTransactions}{Environment.NewLine}" +
                                    $"/// {"Hora Inicio:".PadRight(45)} {processResumen.StartTime:dd-MM-yyyy:HH:mm:ss}{Environment.NewLine}" +
                                    $"/// {"Hora Procesamiento Al Finalizar:".PadRight(45)} {processResumen.EndTime:dd-MM-yyyy:HH:mm:ss}{Environment.NewLine}" +
                                    $"/// {"Tiempo Total Proceso:".PadRight(45)} {processResumen.TotalTimeProcess}{Environment.NewLine}";


            LogMessage(messageToSave, levelLog);
            File.AppendAllText(filePath, messageToSave);
        }

        private void LogMessage(string message, LogLevelCustom levelLog)
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
