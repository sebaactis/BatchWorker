using BatchProcessing.Infraestructure.Database;
using BatchProcessing.Interfaces.Repositories;
using BatchProcessing.Models;
using BatchProcessing.Models.DTO;

namespace BatchProcessing.Repositories
{
    public class ProcessExecutionRepository : IProcessExecutionRepository
    {
        private readonly BatchDbContext _context;

        public ProcessExecutionRepository(BatchDbContext context)
        {
            _context = context;
        }
        public Task<bool> SaveExecution(ProcessExecutionDTO processExecution)
        {
            var executionToSave = new ProcessExecution
            {
                ProcessName = processExecution.ProcessName,
                ProcessStartDate = processExecution.ProcessStartDate,
                ProcessEndDate = processExecution.ProcessEndDate,
                ProcessDuration = processExecution.ProcessDuration,
                ProcessState = processExecution.ProcessState,
                SuccessItems = processExecution.SuccessItems,
                FailedItems = processExecution.FailedItems
            };

            _context.ProcessExecutions.Add(executionToSave);

            try
            {
                _context.SaveChanges();
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al intentar guardar la ejecucion: {ex.Message}");
                return Task.FromResult(false);
            }
        }
    }
}
