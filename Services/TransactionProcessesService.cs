using BatchProcessing.Interfaces;
using BatchProcessing.Models;
using System.Globalization;

namespace BatchProcessing.Services
{
    public class TransactionProcessesService : ITransactionProcessedService<TransactionProcessed>
    {
        private readonly ITransactionINRepository<TransactionRaw> _transactionINRepository;
        private readonly ITransactionProcessesRepository<TransactionProcessed> _transactionProcessesRepository;

        private List<TransactionProcessed> processedTransactions = new List<TransactionProcessed>();

        public TransactionProcessesService(ITransactionINRepository<TransactionRaw> transactionINRepository, ITransactionProcessesRepository<TransactionProcessed> transactionProcessesRepository)
        {
            _transactionINRepository = transactionINRepository;
            _transactionProcessesRepository = transactionProcessesRepository;
        }

        public async Task ProcessesTransactions()
        {
            var transactionsToProcess = _transactionINRepository.FindToProcess();

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
                    // Handle exceptions, log errors, etc.
                    Console.WriteLine($"Error processing transaction {trx.TransactionId}: {ex.Message}");
                }
            }

            if (processedTransactions.Any())
            {
                await _transactionProcessesRepository.Save(processedTransactions);
            }
            else
            {
                Console.WriteLine("No transactions to process.");
            }
        }

        public async Task CreateOUTFile()
        {
            string filePath = "Files\\transactions_output.txt";

            var transactionsToProcess = _transactionProcessesRepository.FindToProcess();

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
                }));

            File.WriteAllLines(filePath, lines);

            foreach (var trx in transactionsToProcess)
                trx.IsConciliated = true;
            

            await _transactionProcessesRepository.SaveChanges();
        }
    }

}
