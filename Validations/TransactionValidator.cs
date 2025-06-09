using BatchProcessing.Models;
using FluentValidation;

namespace BatchProcessing.Validations
{
    public class TransactionValidator : AbstractValidator<TransactionRaw>
    {
        public TransactionValidator()
        {
            RuleFor(x => x.TransactionId).NotEmpty().GreaterThan(0).WithMessage("Transaction ID must be greater than 0 and cannot be null.");
            RuleFor(x => x.MerchantId).NotEmpty().MaximumLength(20).WithMessage("Merchant ID must be greater than 0 and less than 20, and cannot be null.");
            RuleFor(x => x.Currency).NotEmpty().MaximumLength(3).WithMessage("Currency cannot be empty");
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than 0 and cannot be null.");
            RuleFor(x => x.Date).NotEmpty().WithMessage("Date cannot be empty").Must(date => date <= DateTime.Now).WithMessage("Date cannot be in the future.");
        }
    }
}
