using BatchProcessing.Enums;
using BatchProcessing.Interfaces.Services;
using BatchProcessing.Models;
using BatchProcessing.Models.DTO;
using BatchProcessing.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchProcessing.Processes
{
    public static class TransactionOutFileProcess
    {
        public static async Task<long> Execute(ITransactionProcessedService<TransactionProcessed> processesService, ILoggerFileService loggerFileService, IConfiguration configuration, IProcessExecutionService processExecutionService)
        {
            var outFileCreationProcess = await processesService.CreateOUTFile();
            var outFileCreationStartTime = DateTime.Now;
            var outFileCreationTimer = new Stopwatch();
            outFileCreationTimer.Start();

            if (outFileCreationProcess > 0)
            {
                loggerFileService.Log("El archivo OUT se generó correctamente.", LogLevelCustom.Info, "TRANSACTIONS_PROCESSED_CREATE_FILE_OUT");
            }
            else
            {
                loggerFileService.Log("No se pudo generar el archivo OUT.", LogLevelCustom.Error, "TRANSACTIONS_PROCESSED_CREATE_FILE_OUT");
            }

            if (configuration.GetValue<bool>("SaveExecutionProcess"))
            {

                var processExecution = new ProcessExecutionDTO
                {
                    ProcessName = "OUT_FILE_GENERATE",
                    ProcessStartDate = outFileCreationStartTime,
                    ProcessEndDate = DateTime.Now,
                    ProcessDuration = outFileCreationTimer.Elapsed.ToString(),
                    ProcessState = 0,
                    SuccessItems = outFileCreationProcess,
                    FailedItems = 0 // Revisar esto

                };

                outFileCreationTimer.Stop();
                await processExecutionService.SaveExecution(processExecution, "[TRANSACTIONS_OUT_FILE]");

            }

            return outFileCreationProcess;
        }
    }
}
