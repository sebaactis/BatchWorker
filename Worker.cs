using BatchProcessing.Enums;
using BatchProcessing.Interfaces.Services;
using BatchProcessing.Interfaces.Utils;
using BatchProcessing.Interfaces.Validations;
using BatchProcessing.Models;
using BatchProcessing.Models.DTO;
using BatchProcessing.Processes;
using System.Diagnostics;

namespace BatchProcessing
{
    public class Worker : BackgroundService
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public Worker(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var processResume = new DailyResumeDTO();
                var timerFullProcess = new Stopwatch();
                timerFullProcess.Start();

                var reader = scope.ServiceProvider.GetRequiredService<IFileReader<TransactionRaw>>();
                var validator = scope.ServiceProvider.GetRequiredService<ITransactionValidator>();
                var transactionINService = scope.ServiceProvider.GetRequiredService<ITransactionINService<TransactionRaw>>();
                var processesService = scope.ServiceProvider.GetRequiredService<ITransactionProcessedService<TransactionProcessed>>();
                var processExecutionService = scope.ServiceProvider.GetRequiredService<IProcessExecutionService>();
                var loggerFileService = scope.ServiceProvider.GetRequiredService<ILoggerFileService>();

                loggerFileService.Log($"El worker empezo a las {DateTime.Now.ToString("dd-MM-yyyy:HH:mm:ss")}", LogLevelCustom.Info, "");

                var transactions = reader.Read();
                processResume.TotalTransactions = transactions.Count();
                processResume.StartTime = DateTime.Now.ToString("dd-MM-yyyy:HH:mm:ss");

                var transactionsInProcess = await TransactionINProcess.Execute(transactions, loggerFileService, validator, _configuration, transactionINService, processExecutionService);
                processResume.SuccessfulProcessedTransactionsIN = transactionsInProcess.SuccessfulProcessedTransactionsIN;
                processResume.FailedValidationTransactionsIN = transactionsInProcess.FailedValidationTransactionsIN;


                if (_configuration.GetValue<bool>("ProcessingTransactions"))
                {
                    var processTransactions = await TransactionProcessedProcess.Execute(processesService, loggerFileService, _configuration, processExecutionService);

                    processResume.SuccessfulProcessedTransactionsProcessed = processTransactions.successInserts;
                    processResume.FailedValidationTransactionsProcessed = processTransactions.failedValidationInserts;
                    processResume.PermanentFailedTransactions = processTransactions.failedPermanentlyInserts;
                }

                if (_configuration.GetValue<bool>("ReprocessFailedTransactions"))
                {
                    TransactionsReprocessedProcess.Execute(processesService, loggerFileService, _configuration, processExecutionService);
                }


                if (_configuration.GetValue<bool>("GenerateOUTFile"))
                {
                    var outFileCreationProcess = await TransactionOutFileProcess.Execute(processesService, loggerFileService, _configuration, processExecutionService);
                    processResume.TotalOUTTransactions = outFileCreationProcess;
                }

                timerFullProcess.Stop();

                loggerFileService.Log($"El worker de presentacion de transacciones termino a las: {DateTime.Now.ToString("dd-MM-yyyy:HH:mm:ss")}", LogLevelCustom.Info, "");
                loggerFileService.Log($"Proceso terminado en {timerFullProcess}", LogLevelCustom.Info, "");

                processResume.EndTime = DateTime.Now.ToString("dd-MM-yyyy:HH:mm:ss");
                processResume.TotalTimeProcess = timerFullProcess.ToString();

                loggerFileService.LogTotalProcess(processResume, LogLevelCustom.Info);
            }
        }
    }
}
