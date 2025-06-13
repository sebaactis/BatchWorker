using BatchProcessing.Infraestructure.Database;
using BatchProcessing.Interfaces;
using BatchProcessing.Models;
using System.Threading.Tasks;

namespace BatchProcessing.Repositories
{
    public class TransactionProcessesRepository : ITransactionProcessesRepository<TransactionProcessed>
    {

        private readonly BatchDbContext _context;
        public TransactionProcessesRepository(BatchDbContext context)
        {
            _context = context;
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public async Task Save(IEnumerable<TransactionProcessed> transactions)
        {
            if (transactions == null || !transactions.Any())
            {
                throw new ArgumentException("No transactions to save.");
            }

            _context.Transactions_PROCESSED.AddRange(transactions);
             await SaveChanges();
        }

        public IEnumerable<TransactionProcessed> FindToProcess()
        {
            var transactionsToProcess = from trx in _context.Transactions_PROCESSED
                                        where !trx.IsConciliated &&
                                              trx.ProcessedDate <= DateTime.UtcNow
                                        select trx;

            return transactionsToProcess;
        }
    }
}
