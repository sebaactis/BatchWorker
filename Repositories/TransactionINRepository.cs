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
    }
}
