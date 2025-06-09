using BatchProcessing.Interfaces;
using BatchProcessing.Models;
using BatchProcessing.Validations;

namespace BatchProcessing.Services
{
    public class TransactionsValidateService : ITransactionValidator
    {
        private readonly TransactionValidator _validator = new();

        public (List<TransactionRaw> valid, List<(TransactionRaw record, string error)> invalid) Validate(IEnumerable<TransactionRaw> transactions)
        {
            var valid = new List<TransactionRaw>();
            var invalid = new List<(TransactionRaw, string)>();

            foreach (var trx in transactions)
            {
                var result = _validator.Validate(trx);

                if (result.IsValid)
                {
                    valid.Add(trx);
                }
                else
                {
                    invalid.Add((trx, string.Join("; ", result.Errors.Select(e => e.ErrorMessage))));
                }
            }

            return (valid, invalid);
        }
    }
}
