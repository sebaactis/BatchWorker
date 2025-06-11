using BatchProcessing.Interfaces;
using BatchProcessing.Models;
using System.Diagnostics;

namespace BatchProcessing
{
    public class Worker : BackgroundService
    {
        private long transactionsSuccess, transactionsFailed;
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

            _logger.LogInformation("Worker started at: {time}", DateTimeOffset.Now);

            using (var scope = _serviceProvider.CreateScope())
            {
                var reader = scope.ServiceProvider.GetRequiredService<IFileReader<TransactionRaw>>();
                var validator = scope.ServiceProvider.GetRequiredService<ITransactionValidator>();
                var transactionINService = scope.ServiceProvider.GetRequiredService<ITransactionINService<TransactionRaw>>();
                var processesService = scope.ServiceProvider.GetRequiredService<ITransactionProcessedService<TransactionProcessed>>();
                var transactions = reader.Read();

                _logger.LogInformation($"Iniciando el scope para procesar las transacciones | Cantidad de registros a procesar: {transactions.Count()}");

                var validList = new List<TransactionRaw>();

                foreach (var transaction in transactions)
                {
                    _logger.LogInformation($"Procesando la transaccion con ID: {transaction.TransactionId}");

                    var validateTransaction = validator.Validate(new[] { transaction });

                    if (validateTransaction.valid.Any())
                    {
                        validList.Add(transaction);

                        _logger.LogInformation($"Transaction ID: {transaction.TransactionId} | " +
                                               $"Merchant ID: {transaction.MerchantId} | " +
                                               $"Amount: {transaction.Amount} {transaction.Currency} | " +
                                               $"Date: {transaction.Date}");
                        transactionsSuccess++;
                    }
                    else
                    {
                        var transactionErrors = validateTransaction.invalid
                                                .Where(t => t.record.TransactionId == transaction.TransactionId)
                                                .Select(t => t.error)
                                                .ToList();

                        foreach (var error in transactionErrors)
                        {
                            _logger.LogError($"No se pudo grabar la transaccion con ID: {transaction.TransactionId}: Error: {error}");
                        }

                        transactionsFailed++;
                    }
                }

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

                _logger.LogInformation($"Transactions process successfully: {transactionsSuccess}");

                if (transactionsFailed > 0)
                {
                    _logger.LogWarning($"Transactions with errors:  {transactionsFailed}");
                }

                await processesService.ProcessesTransactions();
            }

            timer.Stop();

            _logger.LogInformation("Worker finished processing transactions at: {time}", DateTimeOffset.Now);
            _logger.LogInformation($"Process finished in {timer}");

        }
    }
}
