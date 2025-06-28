using BatchProcessing.Enums;
using BatchProcessing.Interfaces.Services;
using BatchProcessing.Interfaces.Validations;
using BatchProcessing.Models;
using BatchProcessing.Models.DTO;
using BatchProcessing.Services;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace BatchProcessing.Processes
{
    public static class TransactionINProcess
    {
        public async static Task<(long SuccessfulProcessedTransactionsIN, long FailedValidationTransactionsIN)> Execute(IEnumerable<TransactionRaw> transactions, ILoggerFileService loggerFileService, ITransactionValidator validator, IConfiguration configuration, ITransactionINService<TransactionRaw> transactionINService, IProcessExecutionService processExecutionService)
        {
            long transactionsSuccess = 0, transactionsFailed = 0, transactionCount = 1;
            var transactionsINStartTime = DateTime.Now.AddSeconds(5);
            var transactionINTimer = new Stopwatch();
            transactionINTimer.Start();

            loggerFileService.Log($"Iniciando el scope para procesar las transacciones | Cantidad de registros a procesar: {transactions.Count()}", LogLevelCustom.Info, "");

            var validList = new List<TransactionRaw>();

            // Validamos las transacciones para procesarlas.
            foreach (var transaction in transactions)
            {
                loggerFileService.Log($"Procesando la transaccion numero: {transactionCount}", LogLevelCustom.Info, "TRANSACTIONS_IN");

                var validateTransaction = validator.Validate(new[] { transaction });

                if (validateTransaction.valid.Any())
                {
                    validList.Add(transaction);

                    loggerFileService.Log($"Transaction Masked Card: {transaction.CardNumberMasked} | " +
                                           $"Merchant ID: {transaction.MerchantId} | " +
                                           $"Amount: {transaction.Amount} {transaction.Currency} | " +
                                           $"Date: {transaction.Date}", LogLevelCustom.Info, "TRANSACTIONS_IN");
                    transactionsSuccess++;
                    transactionCount++;
                }
                else
                {
                    var transactionErrors = validateTransaction.invalid
                                            .Where(t => t.record.TransactionId == transaction.TransactionId)
                    .Select(t => t.error)
                    .ToList();

                    foreach (var error in transactionErrors)
                    {
                        loggerFileService.Log($"No se pudo grabar la transaccion numero: {transactionCount}: Error: {error}", LogLevelCustom.Warning, "TRANSACTIONS_IN");
                    }

                    transactionsFailed++;
                    transactionCount++;
                }

                await Task.Delay(50);
            }

            // Guardamos las transacciones validas en la base de datos.
            if (validList.Any())
            {
                try
                {
                    var insertedTransactions = await transactionINService.Save(validList, CancellationToken.None);
                    loggerFileService.Log($"Transacciones validas guardadas correctamente: {insertedTransactions}", LogLevelCustom.Info, "TRANSACTIONS_IN");

                    if (configuration.GetValue<bool>("SaveExecutionProcess"))
                    {

                        var processExecution = new ProcessExecutionDTO
                        {
                            ProcessName = "TRANSACTIONS_IN_SAVE",
                            ProcessStartDate = transactionsINStartTime,
                            ProcessEndDate = DateTime.Now,
                            ProcessDuration = transactionINTimer.Elapsed.ToString(),
                            ProcessState = 0,
                            SuccessItems = insertedTransactions,
                            FailedItems = (int)transactionsFailed
                        };

                        transactionINTimer.Stop();
                        await processExecutionService.SaveExecution(processExecution, "[TRANSACTIONS_IN]");

                    }

                    transactionsSuccess = insertedTransactions;
                }
                catch (Exception ex)
                {
                    loggerFileService.Log($"Error al guardar las transacciones validas: {ex}", LogLevelCustom.Error, "TRANSACTIONS_IN");
                    return (0, (int)transactionsFailed);
                }
            }

            loggerFileService.Log($"Transacciones procesadas correctamente: {transactionsSuccess}", LogLevelCustom.Info, "TRANSACTIONS_IN");

            if (transactionsFailed > 0)
            {
                loggerFileService.Log($"Transacciones procesadas correctamente: {transactionsSuccess}", LogLevelCustom.Warning, "TRANSACTIONS_IN");
            }

            return (transactionsSuccess, transactionsFailed);
        }
    }
}
