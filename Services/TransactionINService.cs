using BatchProcessing.Infraestructure.Database;
using BatchProcessing.Interfaces;
using BatchProcessing.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace BatchProcessing.Services
{
    public class TransactionINService : ITransactionINService<TransactionRaw>
    {
        private readonly ITransactionINRepository<TransactionRaw> _repository;
        private readonly BatchDbContext _context;

        public TransactionINService(ITransactionINRepository<TransactionRaw> repository, BatchDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task Save(List<TransactionRaw> transactions, CancellationToken? ct = default)
        {

            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async ct =>
            {
                await using var tx = await _context.Database.BeginTransactionAsync(ct);

                try
                {
                    _context.AddRange(transactions);
                    await _context.SaveChangesAsync(ct);
                    await tx.CommitAsync(ct);
                }
                catch
                {
                    await tx.RollbackAsync(ct);
                    throw;
                }
            }, CancellationToken.None);
        }
    }
}
