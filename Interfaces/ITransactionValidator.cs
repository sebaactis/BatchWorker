using BatchProcessing.Models;

namespace BatchProcessing.Interfaces
{
    public interface ITransactionValidator
    {
        (List<TransactionRaw> valid, List<(TransactionRaw record, string error)> invalid) Validate(IEnumerable<TransactionRaw> transactions);
    }
}
