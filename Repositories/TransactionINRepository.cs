using BatchProcessing.Infraestructure.Database;
using BatchProcessing.Interfaces;
using BatchProcessing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchProcessing.Repositories
{
    public class TransactionINRepository : ITransactionINRepository<TransactionRaw>
    {
        private readonly BatchDbContext _context;

        public TransactionINRepository(BatchDbContext context)
        {
            _context = context;
        }

        public async Task<TransactionRaw> Save(TransactionRaw entity)
        {
            await _context.Transactions_IN.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity;
        }
    }
}
