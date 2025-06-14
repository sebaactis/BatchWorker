using BatchProcessing.Interfaces;
using BatchProcessing.Models;
using System.Globalization;

namespace BatchProcessing.Services
{
    public class TransactionProcessesService : ITransactionProcessedService<TransactionProcessed>
    {
        private readonly ILogger<Worker> _logger;
        private readonly ITransactionINRepository<TransactionRaw> _transactionINRepository;
        private readonly ITransactionProcessesRepository<TransactionProcessed> _transactionProcessesRepository;

        public TransactionProcessesService(ILogger<Worker> logger, ITransactionINRepository<TransactionRaw> transactionINRepository, ITransactionProcessesRepository<TransactionProcessed> transactionProcessesRepository)
        {
            _logger = logger;
            _transactionINRepository = transactionINRepository;
            _transactionProcessesRepository = transactionProcessesRepository;
        }

        public async Task<bool> ProcessesTransactions()
        {
            var transactionsToProcess = _transactionINRepository.FindToProcess();
            var processedTransactions = new List<TransactionProcessed>();

            if (!transactionsToProcess.Any())
            {
                _logger.LogWarning("No se encontraron transacciones para procesar");
                return false;
            }

            foreach (var trx in transactionsToProcess)
            {
                try
                {
                    var processed = new TransactionProcessed
                    {
                        TransactionId = trx.TransactionId,
                        MerchantId = trx.MerchantId,
                        MerchantName = trx.MerchantName,
                        MerchantCountry = trx.MerchantCountry,
                        MerchantCategoryCode = trx.MerchantCategoryCode,
                        CardHolderName = trx.CardHolderName,
                        CardNumberMasked = trx.CardNumberMasked,
                        CardType = trx.CardType,
                        CustomerId = trx.CustomerId,
                        Amount = trx.Amount,
                        AmountLocalCurrency = trx.AmountLocalCurrency,
                        LocalCurrency = trx.LocalCurrency,
                        Currency = trx.Currency,
                        ExchangeRate = trx.ExchangeRate,
                        Date = trx.Date,
                        PostingDate = trx.PostingDate,
                        AuthorizationDate = trx.AuthorizationDate,
                        TerminalId = trx.TerminalId,
                        POSLocation = trx.POSLocation,
                        POSCountryCode = trx.POSCountryCode,
                        EntryMode = trx.EntryMode,
                        AuthorizationCode = trx.AuthorizationCode,
                        TransactionType = trx.TransactionType,
                        Status = trx.Status,
                        IsInternational = trx.IsInternational,
                        IsFraudSuspected = trx.IsFraudSuspected,
                        IsOfflineTransaction = trx.IsOfflineTransaction,
                        IsConciliated = false,
                        Notes = trx.Notes,
                        ReferenceNumber = trx.ReferenceNumber,
                        ProcessedDate = DateTime.UtcNow
                    };

                    processedTransactions.Add(processed);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error procesando la transacción {trx.TransactionId}");
                }
            }

            if (processedTransactions.Any())
            {
                try
                {
                    await _transactionProcessesRepository.Save(processedTransactions);
                    _logger.LogInformation($"Se procesaron correctamente y se guardaron {processedTransactions.Count()} transacciones.");
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al guardar las transacciones procesadas");
                    return false;
                }

            }
            else
            {
                _logger.LogWarning("Ninguna transacción fue procesada correctamente.");
                return false;
            }
        }

        public async Task<bool> CreateOUTFile()
        {
            _logger.LogInformation("Iniciando proceso para generar archivo OUT");
            string filePath = "Files\\transactions_output.txt";

            var transactionsToProcess = _transactionProcessesRepository.FindToProcess();

            if (!transactionsToProcess.Any())
            {
                Console.WriteLine(transactionsToProcess.Count());
                _logger.LogWarning("No hay transacciones para procesar en el archivo OUT");
                return false;
            }

            try
            {
                var lines = transactionsToProcess.Select(trx =>
                string.Join("000", new[]
                {
                    trx.TransactionId.ToString(),
                    trx.MerchantId,
                    trx.MerchantName,
                    trx.MerchantCountry,
                    trx.MerchantCategoryCode,
                    trx.CardHolderName,
                    trx.CardNumberMasked,
                    trx.CardType,
                    trx.CustomerId,
                    trx.Amount.ToString("F2", CultureInfo.InvariantCulture),
                    trx.AmountLocalCurrency.ToString("F2", CultureInfo.InvariantCulture),
                    trx.Currency,
                    trx.LocalCurrency,
                    trx.ExchangeRate.ToString("F4", CultureInfo.InvariantCulture),
                    trx.Date.ToString("yyyy-MM-dd"),
                    trx.PostingDate.ToString("yyyy-MM-dd"),
                    trx.AuthorizationDate.ToString("yyyy-MM-dd"),
                    trx.TerminalId,
                    trx.POSLocation,
                    trx.POSCountryCode,
                    trx.EntryMode,
                    trx.AuthorizationCode,
                    trx.TransactionType,
                    trx.Status,
                    trx.Notes,
                    trx.ReferenceNumber,
                    trx.ProcessedDate.ToString("yyyy-MM-dd HH:mm:ss")
                })).ToList();

                File.WriteAllLines(filePath, lines);

                _logger.LogInformation("Archivo OUT generado correctamente en: {filePath}", filePath);

                foreach (var trx in transactionsToProcess)
                {
                    trx.IsConciliated = true;
                    trx.ConciliatedDate = DateTime.Now;
                }

                await _transactionProcessesRepository.SaveChanges();

                _logger.LogInformation("Transacciones marcadas como conciliadas correctamente.");
                _logger.LogInformation($"Cantidad de transaccion conciliadas: {lines.Count()}");
                return true;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la creacion del archivo OUT");
                return false;
            }
        }
    }
}
