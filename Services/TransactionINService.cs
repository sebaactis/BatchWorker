using BatchProcessing.Infraestructure.Database;
using BatchProcessing.Interfaces;
using BatchProcessing.Models;

namespace BatchProcessing.Services
{
    public class TransactionINService : ITransactionINService<TransactionRaw>
    {
        private readonly ITransactionINRepository<TransactionRaw> _repository;

        public TransactionINService(ITransactionINRepository<TransactionRaw> repository)
        {
            _repository = repository;
        }

        public async Task<TransactionRaw> Save(TransactionRaw transaction)
        {
            return await _repository.Save(transaction);
        }
    }
}
