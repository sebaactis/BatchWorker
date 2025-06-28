using BatchProcessing.Models.DTO;

namespace BatchProcessing.Interfaces.Services
{
    public interface IProcessExecutionService
    {
        Task SaveExecution(ProcessExecutionDTO processExecution, string methodCalled);
    }
}
