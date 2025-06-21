using BatchProcessing.Enums;
using BatchProcessing.Interfaces;
using BatchProcessing.Models;
using BatchProcessing.Models.DTO;
using System.Diagnostics;

namespace BatchProcessing
{
    public class Worker : BackgroundService
    {
        private long transactionsSuccess, transactionsFailed, transactionCount = 1;
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
                var timer = new Stopwatch();
                timer.Start();

                var reader = scope.ServiceProvider.GetRequiredService<IFileReader<TransactionRaw>>();
                var validator = scope.ServiceProvider.GetRequiredService<ITransactionValidator>();
                var transactionINService = scope.ServiceProvider.GetRequiredService<ITransactionINService<TransactionRaw>>();
                var processesService = scope.ServiceProvider.GetRequiredService<ITransactionProcessedService<TransactionProcessed>>();
                var loggerFileService = scope.ServiceProvider.GetRequiredService<ILoggerFileService>();
                var transactions = reader.Read();

                processResume.TotalTransactions = transactions.Count();

                loggerFileService.Log($"El worker empezo a las {DateTime.Now.ToString("dd-MM-yyyy:HH:mm:ss")}", LogLevelCustom.Info, "");
                processResume.StartTime = DateTime.Now.ToString("dd-MM-yyyy:HH:mm:ss");

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
                        processResume.FailedValidationTransactionsIN++;
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
                        processResume.SuccessfulProcessedTransactionsIN = insertedTransactions;
                    }
                    catch (Exception ex)
                    {
                        loggerFileService.Log($"Error al guardar las transacciones validas: {ex}", LogLevelCustom.Error, "TRANSACTIONS_IN");
                    }
                }

                loggerFileService.Log($"Transacciones procesadas correctamente: {transactionsSuccess}", LogLevelCustom.Info, "TRANSACTIONS_IN");

                if (transactionsFailed > 0)
                {
                    loggerFileService.Log($"Transacciones procesadas correctamente: {transactionsSuccess}", LogLevelCustom.Warning, "TRANSACTIONS_IN");
                }


                if (_configuration.GetValue<bool>("ProcessingTransactions"))
                {
                    // Proceso de transaccion de Transactions_IN a Transactions_Processed
                    var processTransactions = await processesService.ProcessesTransactions();

                    if (processTransactions.successInserts > 0)
                    {
                        loggerFileService.Log("El proceso de transacciones finalizó correctamente.", LogLevelCustom.Info, "TRANSACTIONS_PROCESSED");
                    }
                    else
                    {
                        loggerFileService.Log("El proceso de transacciones no se completó con éxito.", LogLevelCustom.Warning, "TRANSACTIONS_PROCESSED");
                    }

                    processResume.SuccessfulProcessedTransactionsProcessed = processTransactions.successInserts;
                    processResume.FailedValidationTransactionsProcessed = processTransactions.failedValidationInserts;
                    processResume.PermanentFailedTransactions = processTransactions.failedPermanentlyInserts;
                }

                if (_configuration.GetValue<bool>("GenerateOUTFile"))
                {
                    // Generamos el archivo de salida.
                    var OutFileCreationProcess = await processesService.CreateOUTFile();

                    if (OutFileCreationProcess > 0)
                    {
                        loggerFileService.Log("El archivo OUT se generó correctamente.", LogLevelCustom.Info, "TRANSACTIONS_PROCESSED_CREATE_FILE_OUT");
                    }
                    else
                    {
                        loggerFileService.Log("No se pudo generar el archivo OUT.", LogLevelCustom.Error, "TRANSACTIONS_PROCESSED_CREATE_FILE_OUT");
                    }

                    processResume.TotalOUTTransactions = OutFileCreationProcess;
                }


                timer.Stop();
                loggerFileService.Log($"El worker de presentacion de transacciones termino a las: {DateTime.Now.ToString("dd-MM-yyyy:HH:mm:ss")}", LogLevelCustom.Info, "");
                loggerFileService.Log($"Proceso terminado en {timer}", LogLevelCustom.Info, "");

                processResume.EndTime = DateTime.Now.ToString("dd-MM-yyyy:HH:mm:ss");
                processResume.TotalTimeProcess = timer.ToString();
                loggerFileService.LogTotalProcess(processResume, LogLevelCustom.Info);
            }
        }
    }
}
