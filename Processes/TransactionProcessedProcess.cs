
using BatchProcessing.Enums;
using BatchProcessing.Interfaces.Services;
using BatchProcessing.Models;
using BatchProcessing.Models.DTO;
using BatchProcessing.Services;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace BatchProcessing.Processes
{
    public static class TransactionProcessedProcess
    {
        public static async Task<(long successInserts, long failedValidationInserts, long failedPermanentlyInserts)> Execute(ITransactionProcessedService<TransactionProcessed> transactionProcessedService, ILoggerFileService loggerFileService, IConfiguration configuration, IProcessExecutionService processExecutionService)
        {
            var processTransactions = await transactionProcessedService.ProcessesTransactions();
            var processingTransactionsStartTime = DateTime.Now;
            var processingTransactionsTimer = new Stopwatch();
            processingTransactionsTimer.Start();

            if (processTransactions.successInserts > 0)
            {
                loggerFileService.Log("El proceso de transacciones finalizó correctamente.", LogLevelCustom.Info, "TRANSACTIONS_PROCESSED");

                if (configuration.GetValue<bool>("SaveExecutionProcess"))
                {

                    var processExecution = new ProcessExecutionDTO
                    {
                        ProcessName = "TRANSACTIONS_PROCESSED",
                        ProcessStartDate = processingTransactionsStartTime,
                        ProcessEndDate = DateTime.Now,
                        ProcessDuration = processingTransactionsTimer.Elapsed.ToString(),
                        ProcessState = 0,
                        SuccessItems = processTransactions.successInserts,
                        FailedItems = processTransactions.failedValidationInserts + processTransactions.failedPermanentlyInserts

                    };

                    processingTransactionsTimer.Stop();
                    await processExecutionService.SaveExecution(processExecution, "[TRANSACTIONS_PROCESSED]");

                }
            }
            else
            {
                loggerFileService.Log("El proceso de transacciones no se completó con éxito.", LogLevelCustom.Warning, "TRANSACTIONS_PROCESSED");
            }

            return (processTransactions.successInserts, processTransactions.failedValidationInserts, processTransactions.failedPermanentlyInserts);
        }
    }
}
