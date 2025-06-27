using BatchProcessing.Infraestructure.Database;
using BatchProcessing.Interfaces;
using BatchProcessing.Models;

namespace BatchProcessing.Repositories
{
    public class TransactionINRepository : ITransactionINRepository<TransactionRaw>
    {
        private readonly BatchDbContext _context;

        public TransactionINRepository(BatchDbContext context)
        {
            _context = context;
        }

        public void AddRange(IEnumerable<TransactionRaw> entities)
        => _context.Transactions_IN.AddRange(entities);

        public IEnumerable<TransactionRaw> FindToProcess()
        {
            var transactions = from trx in _context.Transactions_IN
                               where !trx.IsProcessed && trx.Status == "Approved" && !trx.FailedPermanently && trx.RetryCount < 3
                               select trx;

            return transactions;
        }

        public IEnumerable<TransactionRaw> FindToReprocessPermanently()
        {
            var transactions = from trx in _context.Transactions_IN
                               where trx.FailedPermanently && trx.RetryCount >= 3 && !trx.IsProcessed
                               select trx;

            return transactions;
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar los cambios en la base de datos", ex);
            }
        }
    }
}
