using BatchProcessing.Interfaces;
using BatchProcessing.Models;
using BatchProcessing.Services;
using System.Diagnostics;

namespace BatchProcessing
{
    public class Worker : BackgroundService
    {
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

                var transactions = reader.Read();
                var validation = validator.Validate(transactions);

                foreach (var transaction in transactions)
                {
                    await transactionINService.Save(transaction);

                    _logger.LogInformation($"Transaction ID: {transaction.TransactionId} | " +
                                           $"Merchant ID: {transaction.MerchantId} | " +
                                           $"Amount: {transaction.Amount} {transaction.Currency} | " +
                                           $"Date: {transaction.Date}");

                    Thread.Sleep(500);

                    var transactionErrors = validation.invalid
                                            .Where(t => t.record.TransactionId == transaction.TransactionId)
                                            .Select(t => t.error)
                                            .ToList();

                    if (transactionErrors.Any())
                    {
                        foreach (var error in transactionErrors)
                        {
                            _logger.LogWarning($"Error on transaction ID {transaction.TransactionId}: Error: {error}");
                        }
                    }
                }

                _logger.LogInformation($"Transactions process successfully: {validation.valid.Count()}");

                if (validation.invalid.Count > 0)
                {
                    _logger.LogWarning($"Transactions with error, please check: {validation.invalid.Count()}");
                }
            }

            timer.Stop();

            _logger.LogInformation("Worker finished processing transactions at: {time}", DateTimeOffset.Now);
            _logger.LogInformation($"Process finished in {timer}");

        }
    }
}
