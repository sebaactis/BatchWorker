using BatchProcessing.Enums;
using BatchProcessing.Interfaces.Repositories;
using BatchProcessing.Interfaces.Services;
using BatchProcessing.Models.DTO;
using BatchProcessing.Validations;

namespace BatchProcessing.Services
{
    public class ProcessExecutionService : IProcessExecutionService
    {
        private readonly IProcessExecutionRepository _processExecutionRepository;
        private readonly ILoggerFileService _loggerFileService;

        public ProcessExecutionService(IProcessExecutionRepository processExecutionRepository, ILoggerFileService loggerFileService)
        {
            _processExecutionRepository = processExecutionRepository;
            _loggerFileService = loggerFileService;
        }
        public async Task SaveExecution(ProcessExecutionDTO processExecution)
        {
            var validator = new ProcessExecutionValidator();
            var validationResult = validator.Validate(processExecution);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    _loggerFileService.Log($"Error en la validacion al grabar la ejecucion del proceso en la propiedad: {error.PropertyName}. Mensaje: {error.ErrorMessage}", LogLevelCustom.Error, "PROCESS_EXECUTIONS");
                }
            }

            try
            {
                var execution = await _processExecutionRepository.SaveExecution(processExecution);

                if (!execution)
                {
                    _loggerFileService.Log("No se pudo guardar la ejecucion del proceso", LogLevelCustom.Error, "PROCESS_EXECUTIONS");
                }

                _loggerFileService.Log("Ejecucion del proceso guardada correctamente", LogLevelCustom.Info, "PROCESS_EXECUTIONS");
            }
            catch (Exception ex)
            {
                _loggerFileService.Log("Error al ejecutar el proceso", LogLevelCustom.Error, "PROCESS_EXECUTIONS");
            }
        }
    }
}
