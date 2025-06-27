using BatchProcessing.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchProcessing.Interfaces.Repositories
{
    public interface IProcessExecutionRepository
    {
        Task<bool> SaveExecution(ProcessExecutionDTO processExecution);
    }
}
