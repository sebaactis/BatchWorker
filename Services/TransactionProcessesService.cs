using BatchProcessing.Enums;
using BatchProcessing.Interfaces.Repositories;
using BatchProcessing.Interfaces.Services;
using BatchProcessing.Models;
using FluentValidation;
using System.Diagnostics;
using System.Globalization;

namespace BatchProcessing.Services
{
    public class TransactionProcessesService : ITransactionProcessedService<TransactionProcessed>
    {
        private readonly ILoggerFileService _loggerFileService;
        private readonly ITransactionINRepository<TransactionRaw> _transactionINRepository;
        private readonly ITransactionProcessesRepository<TransactionProcessed> _transactionProcessesRepository;
        private readonly IValidator<TransactionProcessed> _validator;

        public TransactionProcessesService(ILoggerFileService loggerFileService, ITransactionINRepository<TransactionRaw> transactionINRepository, ITransactionProcessesRepository<TransactionProcessed> transactionProcessesRepository, IValidator<TransactionProcessed> validator)
        {
            _validator = validator;
            _loggerFileService = loggerFileService;
            _transactionINRepository = transactionINRepository;
            _transactionProcessesRepository = transactionProcessesRepository;
        }

        public async Task<(int successInserts, int failedPermanentlyInserts, int failedValidationInserts)> ProcessesTransactions()
        {

            var transactionsToProcess = _transactionINRepository.FindToProcess();

            if (!transactionsToProcess.Any())
            {
                _loggerFileService.Log("No se encontraron transacciones para procesar", LogLevelCustom.Warning, "TRANSACTIONS_PROCESSED");
                return (0, 0, 0);
            }

            var (processedList, success, failedValidation, failedPermanent) = await ProcessTransactionsInternal(transactionsToProcess.ToList(), "TRANSACTIONS_PROCESSED");

            if (processedList.Any())
            {
                try
                {
                    await _transactionProcessesRepository.Save(processedList);
                    _loggerFileService.Log($"Se procesaron correctamente y se guardaron {processedList.Count} transacciones.", LogLevelCustom.Info, "TRANSACTIONS_PROCESSED");
                }
                catch (Exception ex)
                {
                    _loggerFileService.Log($"Error al guardar las transacciones procesadas: {ex}", LogLevelCustom.Error, "TRANSACTIONS_PROCESSED");
                    return (0, 0, 0);
                }
            }

            return (success, failedPermanent, failedValidation);
        }

        public async Task<(int successProcessed, int failedProcessed)> ReprocessedFailedPermanentlyTransactions()
        {
            var transactionsToProcess = _transactionINRepository.FindToReprocessPermanently();

            if (!transactionsToProcess.Any())
            {
                _loggerFileService.Log("No hay transacciones para re-procesar permanentemente", LogLevelCustom.Warning, "TRANSACTIONS_PROCESSED_REPROCESS_PERMANENTLY");
                return (0, 0);
            }

            var (processedList, success, failedValidation, failedPermanent) =
                await ProcessTransactionsInternal(transactionsToProcess.ToList(), "TRANSACTIONS_PROCESSED_REPROCESS_PERMANENTLY", resetFailedPermanently: true);

            if (processedList.Any())
            {
                try
                {
                    await _transactionProcessesRepository.Save(processedList);
                    _loggerFileService.Log($"Se procesaron correctamente y se guardaron {processedList.Count} transacciones.", LogLevelCustom.Info, "TRANSACTIONS_PROCESSED_REPROCESS_PERMANENTLY");
                }
                catch (Exception ex)
                {
                    _loggerFileService.Log($"Error al guardar las transacciones procesadas: {ex}", LogLevelCustom.Error, "TRANSACTIONS_PROCESSED_REPROCESS_PERMANENTLY");
                    return (0, failedPermanent);
                }
            }

            return (success, failedPermanent);
        }

        public async Task<int> CreateOUTFile()
        {
            _loggerFileService.Log("Iniciando proceso para generar archivo OUT", LogLevelCustom.Info, "TRANSACTIONS_PROCESSED_CREATE_FILE_OUT");
            string filePath = "Files\\transactions_output.txt";

            var transactionsToProcess = _transactionProcessesRepository.FindToProcess();

            if (!transactionsToProcess.Any())
            {
                _loggerFileService.Log("No hay transacciones para procesar en el archivo OUT", LogLevelCustom.Warning, "TRANSACTIONS_PROCESSED_CREATE_FILE_OUT");
                return 0;
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

                _loggerFileService.Log($"Archivo OUT generado correctamente en: {filePath}", LogLevelCustom.Info, "TRANSACTIONS_PROCESSED_CREATE_FILE_OUT");

                foreach (var trx in transactionsToProcess)
                {
                    trx.IsConciliated = true;
                    trx.ConciliatedDate = DateTime.Now;
                }

                await _transactionProcessesRepository.SaveChanges();

                _loggerFileService.Log("Transacciones marcadas como conciliadas correctamente.", LogLevelCustom.Info, "TRANSACTIONS_PROCESSED_CREATE_FILE_OUT");
                _loggerFileService.Log($"Cantidad de transacciones conciliadas: {lines.Count()}", LogLevelCustom.Info, "TRANSACTIONS_PROCESSED_CREATE_FILE_OUT");
                return lines.Count();
            }

            catch (Exception ex)
            {
                _loggerFileService.Log($"Error durante la creacion del archivo OUT: {ex}", LogLevelCustom.Error, "TRANSACTIONS_PROCESSED_CREATE_FILE_OUT");
                return 0;
            }
        }


        private async Task<(List<TransactionProcessed> processedList, int success, int failedValidation, int failedPermanent)> ProcessTransactionsInternal(
    List<TransactionRaw> transactions, string logScope, bool resetFailedPermanently = false)
        {
            var processedList = new List<TransactionProcessed>();
            int success = 0, failedValidation = 0, failedPermanent = 0;

            foreach (var trx in transactions)
            {
                if (resetFailedPermanently)
                {
                    trx.RetryCount = 0;
                    trx.FailedPermanently = false;
                }

                int attempt = 0;
                bool done = false;

                while (attempt < 3 && !done)
                {
                    try
                    {
                        var (isValid, processed, errors) = TryProcessTransaction(trx);

                        if (!isValid)
                        {
                            foreach (var err in errors!)
                            {
                                _loggerFileService.Log($"Error de validación en transacción {trx.TransactionId}: {err}", LogLevelCustom.Warning, logScope);
                            }

                            trx.RetryCount++;
                            attempt++;
                            failedValidation++;

                            if (attempt >= 3)
                            {
                                trx.FailedPermanently = true;
                                failedPermanent++;
                                _loggerFileService.Log($"Transacción inválida permanentemente tras 3 intentos. ID: {trx.TransactionId}", LogLevelCustom.Error, logScope);
                            }

                            continue;
                        }

                        processedList.Add(processed!);
                        trx.IsProcessed = true;
                        success++;
                        _loggerFileService.Log($"Transaccion procesada OK: {processed?.TransactionId}", LogLevelCustom.Info, logScope);
                        done = true;
                    }
                    catch (Exception ex)
                    {
                        attempt++;
                        trx.RetryCount++;

                        _loggerFileService.Log($"Intento {attempt} fallido para transacción {trx.TransactionId}. Error: {ex.Message}", LogLevelCustom.Warning, logScope);

                        if (attempt >= 3)
                        {
                            trx.FailedPermanently = true;
                            failedPermanent++;
                            _loggerFileService.Log($"Transacción {trx.TransactionId} ha fallado 3 veces, se marca como fallida permanentemente", LogLevelCustom.Error, logScope);
                        }

                        await Task.Delay(2000);
                    }
                }
            }

            await _transactionINRepository.SaveChangesAsync();

            return (processedList, success, failedValidation, failedPermanent);
        }


        private (bool isValid, TransactionProcessed? processed, string[]? validationErrors) TryProcessTransaction(TransactionRaw trx)
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
                IsConciliated = false, // Por defecto, no está conciliada
                Notes = trx.Notes,
                ReferenceNumber = trx.ReferenceNumber,
                ProcessedDate = DateTime.UtcNow
            };

            var validationResult = _validator.Validate(processed);

            if (!validationResult.IsValid)
            {
                return (false, null, validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
            }

            return (true, processed, null);
        }
    }
}
