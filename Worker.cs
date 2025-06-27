using BatchProcessing.Enums;
using BatchProcessing.Interfaces.Services;
using BatchProcessing.Interfaces.Utils;
using BatchProcessing.Interfaces.Validations;
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
                var timerFullProcess = new Stopwatch();
                timerFullProcess.Start();

                var reader = scope.ServiceProvider.GetRequiredService<IFileReader<TransactionRaw>>();
                var validator = scope.ServiceProvider.GetRequiredService<ITransactionValidator>();
                var transactionINService = scope.ServiceProvider.GetRequiredService<ITransactionINService<TransactionRaw>>();
                var processesService = scope.ServiceProvider.GetRequiredService<ITransactionProcessedService<TransactionProcessed>>();
                var processExecutionService = scope.ServiceProvider.GetRequiredService<IProcessExecutionService>();
                var loggerFileService = scope.ServiceProvider.GetRequiredService<ILoggerFileService>();


                var transactions = reader.Read();
                var transactionsINStartTime = DateTime.Now;
                var transactionINTimer = new Stopwatch();
                transactionINTimer.Start();

                processResume.TotalTransactions = transactions.Count();
                processResume.StartTime = DateTime.Now.ToString("dd-MM-yyyy:HH:mm:ss");

                loggerFileService.Log($"El worker empezo a las {DateTime.Now.ToString("dd-MM-yyyy:HH:mm:ss")}", LogLevelCustom.Info, "");
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

                        if (_configuration.GetValue<bool>("SaveExecutionProcess"))
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
                            await processExecutionService.SaveExecution(processExecution);

                        }
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
                    var processingTransactionsStartTime = DateTime.Now;
                    var processingTransactionsTimer = new Stopwatch();
                    processingTransactionsTimer.Start();

                    if (processTransactions.successInserts > 0)
                    {
                        loggerFileService.Log("El proceso de transacciones finalizó correctamente.", LogLevelCustom.Info, "TRANSACTIONS_PROCESSED");

                        if (_configuration.GetValue<bool>("SaveExecutionProcess"))
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
                            await processExecutionService.SaveExecution(processExecution);

                        }
                    }
                    else
                    {
                        loggerFileService.Log("El proceso de transacciones no se completó con éxito.", LogLevelCustom.Warning, "TRANSACTIONS_PROCESSED");
                    }

                    processResume.SuccessfulProcessedTransactionsProcessed = processTransactions.successInserts;
                    processResume.FailedValidationTransactionsProcessed = processTransactions.failedValidationInserts;
                    processResume.PermanentFailedTransactions = processTransactions.failedPermanentlyInserts;

                    if (_configuration.GetValue<bool>("ReprocessFailedTransactions"))
                    {
                        var reprocessTransactions = await processesService.ReprocessedFailedPermanentlyTransactions();
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

                        if (_configuration.GetValue<bool>("SaveExecutionProcess"))
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
                            await processExecutionService.SaveExecution(processExecution);

                        }
                    }
                }



                if (_configuration.GetValue<bool>("GenerateOUTFile"))
                {
                    // Generamos el archivo de salida.
                    var outFileCreationProcess = await processesService.CreateOUTFile();
                    var outFileCreationStartTime = DateTime.Now;
                    var outFileCreationTimer = new Stopwatch();
                    outFileCreationTimer.Start();

                    if (outFileCreationProcess > 0)
                    {
                        loggerFileService.Log("El archivo OUT se generó correctamente.", LogLevelCustom.Info, "TRANSACTIONS_PROCESSED_CREATE_FILE_OUT");
                    }
                    else
                    {
                        loggerFileService.Log("No se pudo generar el archivo OUT.", LogLevelCustom.Error, "TRANSACTIONS_PROCESSED_CREATE_FILE_OUT");
                    }

                    if (_configuration.GetValue<bool>("SaveExecutionProcess"))
                    {

                        var processExecution = new ProcessExecutionDTO
                        {
                            ProcessName = "OUT_FILE_GENERATE",
                            ProcessStartDate = outFileCreationStartTime,
                            ProcessEndDate = DateTime.Now,
                            ProcessDuration = outFileCreationTimer.Elapsed.ToString(),
                            ProcessState = 0,
                            SuccessItems = outFileCreationProcess,
                            FailedItems = 0 // Revisar esto

                        };

                        outFileCreationTimer.Stop();
                        await processExecutionService.SaveExecution(processExecution);

                    }

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
