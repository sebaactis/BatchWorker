using BatchProcessing.Enums;
using BatchProcessing.Interfaces.Services;
using BatchProcessing.Models;
using BatchProcessing.Models.DTO;
using BatchProcessing.Services;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace BatchProcessing.Processes
{
    public static class TransactionsReprocessedProcess
    {
        public static async void Execute(ITransactionProcessedService<TransactionProcessed> processedService, ILoggerFileService loggerFileService, IConfiguration configuration, IProcessExecutionService processExecutionService)
        {
            var reprocessTransactions = await processedService.ReprocessedFailedPermanentlyTransactions();
            var reprocessingTransactionsStartTime = DateTime.Now;
            var reprocessingTransactionsTimer = new Stopwatch();
            reprocessingTransactionsTimer.Start();

            if (reprocessTransactions.successProcessed > 0)
            {
                loggerFileService.Log("El re-proceso de transacciones fallidas se completó correctamente.", LogLevelCustom.Info, "TRANSACTIONS_PROCESSED_REPROCESS_PERMANENTLY");
            }
            else
            {
                loggerFileService.Log("El re-proceso de transacciones fallidas no tuvo transacciones para reprocesar.", LogLevelCustom.Warning, "TRANSACTIONS_PROCESSED_REPROCESS_PERMANENTLY");
            }

            if (configuration.GetValue<bool>("SaveExecutionProcess"))
            {

                var processExecution = new ProcessExecutionDTO
                {
                    ProcessName = "TRANSACTIONS_REPROCESSED",
                    ProcessStartDate = reprocessingTransactionsStartTime,
                    ProcessEndDate = DateTime.Now,
                    ProcessDuration = reprocessingTransactionsTimer.Elapsed.ToString(),
                    ProcessState = 0,
                    SuccessItems = reprocessTransactions.successProcessed,
                    FailedItems = reprocessTransactions.failedProcessed

                };

                reprocessingTransactionsTimer.Stop();
                await processExecutionService.SaveExecution(processExecution, "[TRANSACTIONS_REPROCESSED]");

            }
        }
    }
}
