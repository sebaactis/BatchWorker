
using BatchProcessing.Models.DTO;
using FluentValidation;

namespace BatchProcessing.Validations
{
    public class ProcessExecutionValidator : AbstractValidator<ProcessExecutionDTO>
    {
        public ProcessExecutionValidator()
        {
            RuleFor(x => x.ProcessName)
                .NotEmpty()
                .WithMessage("El nombre del proceso no puede estar vacio")
                .MaximumLength(30)
                .WithMessage("El nombre del proceso no puede superar los caracteres");

            RuleFor(x => x.ProcessStartDate)
                .GreaterThanOrEqualTo(x => DateTime.Now.AddSeconds(-5))
                .WithMessage("La fecha de inicio no puede ser menor a la fecha actual");

            RuleFor(x => x.ProcessEndDate)
                .GreaterThanOrEqualTo(x => DateTime.Now.AddSeconds(-5))
                .WithMessage("La fecha de finalización no puede ser menor a la fecha actual");

            RuleFor(x => x.ProcessDuration)
                .NotEmpty()
                .WithMessage("La duracion del proceso no puede estar vacia")
                .MaximumLength(20)
                .WithMessage("La duracion del proceso no puede superar los 20 caracteres");

            RuleFor(x => x.ProcessState)
                .InclusiveBetween(0, 1)
                .WithMessage("El estado del proceso debe ser 0 (Finalizado), 1 (Finalizado con error)");

            RuleFor(x => x.SuccessItems)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El numero de items exitosos debe ser mayor o igual a 0");

            RuleFor(x => x.FailedItems)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El numero de items fallidos debe ser mayor o igual a 0");
        }
    }

}

