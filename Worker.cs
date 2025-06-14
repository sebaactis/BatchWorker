using BatchProcessing.Interfaces;
using BatchProcessing.Models;
using System.Diagnostics;

namespace BatchProcessing
{
    public class Worker : BackgroundService
    {
        private long transactionsSuccess, transactionsFailed, transactionCount = 1;
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;


        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var timer = new Stopwatch();
            timer.Start();

            _logger.LogInformation("El worker empezo a las: {time}", DateTimeOffset.Now);

            using (var scope = _serviceProvider.CreateScope())
            {
                var reader = scope.ServiceProvider.GetRequiredService<IFileReader<TransactionRaw>>();
                var validator = scope.ServiceProvider.GetRequiredService<ITransactionValidator>();
                var transactionINService = scope.ServiceProvider.GetRequiredService<ITransactionINService<TransactionRaw>>();
                var processesService = scope.ServiceProvider.GetRequiredService<ITransactionProcessedService<TransactionProcessed>>();
                var transactions = reader.Read();

                _logger.LogInformation($"Iniciando el scope para procesar las transacciones | Cantidad de registros a procesar: {transactions.Count()}");

                var validList = new List<TransactionRaw>();

                // Validamos las transacciones para procesarlas.
                foreach (var transaction in transactions)
                {
                    _logger.LogInformation($"Procesando la transaccion numero: {transactionCount}");

                    var validateTransaction = validator.Validate(new[] { transaction });

                    if (validateTransaction.valid.Any())
                    {
                        validList.Add(transaction);

                        _logger.LogInformation($"Transaction Masked Card: {transaction.CardNumberMasked} | " +
                                               $"Merchant ID: {transaction.MerchantId} | " +
                                               $"Amount: {transaction.Amount} {transaction.Currency} | " +
                                               $"Date: {transaction.Date}");
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
                            _logger.LogError($"No se pudo grabar la transaccion numero: {transactionCount}: Error: {error}");
                        }

                        transactionsFailed++;
                        transactionCount++;
                    }
                    Thread.Sleep(50);
                }

                // Guardamos las transacciones validas en la base de datos.
                if (validList.Any())
                {
                    try
                    {
                        await transactionINService.Save(validList, CancellationToken.None);
                        _logger.LogInformation($"Transacciones validas guardadas correctamente: {validList.Count}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error al guardar las transacciones validas");
                    }
                }

                _logger.LogInformation($"Transacciones procesadas correctamente: {transactionsSuccess}");

                if (transactionsFailed > 0)
                {
                    _logger.LogWarning($"Transacciones con errores:  {transactionsFailed}");
                }

                // Proceso de transaccion de Transactions_IN a Transactions_Processed
                var processTransactions = await processesService.ProcessesTransactions();

                if (processTransactions)
                {
                    _logger.LogInformation("El proceso de transacciones finalizó correctamente.");
                }
                else
                {
                    _logger.LogWarning("El proceso de transacciones no se completó con éxito.");
                }

                // Generamos el archivo de salida.
                var OutFileCreationProcess = await processesService.CreateOUTFile();

                if (OutFileCreationProcess)
                {
                    _logger.LogInformation("El archivo OUT se generó correctamente.");
                }
                else
                {
                    _logger.LogWarning("No se pudo generar el archivo OUT.");
                }
            }

            timer.Stop();

            _logger.LogInformation("El worker de presentacion de transacciones termino en el tiempo de: {time}", DateTimeOffset.Now);
            _logger.LogInformation($"Proceso terminado en {timer}");

        }
    }
}
