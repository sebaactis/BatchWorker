using BatchProcessing.Interfaces;
using BatchProcessing.Models;

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
    }

}
