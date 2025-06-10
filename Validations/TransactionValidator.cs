using BatchProcessing.Models;
using FluentValidation;

public class TransactionValidator : AbstractValidator<TransactionRaw>
{
    public TransactionValidator()
    {
        RuleFor(x => x.TransactionId)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("Transaction ID must be greater than 0 and cannot be null.");

        RuleFor(x => x.MerchantId)
            .NotEmpty()
            .MaximumLength(20)
            .WithMessage("Merchant ID must be between 1 and 20 characters and cannot be null.");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .MaximumLength(3)
            .WithMessage("Currency must be exactly 3 characters and cannot be empty.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than 0 and cannot be null.");

        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("Date cannot be empty")
            .Must(date => date <= DateTime.Now)
            .WithMessage("Date cannot be in the future.");

        RuleFor(x => x.MerchantName)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.MerchantName))
            .WithMessage("Merchant name cannot exceed 100 characters.");

        RuleFor(x => x.MerchantCountry)
            .MaximumLength(3)
            .When(x => !string.IsNullOrEmpty(x.MerchantCountry))
            .WithMessage("Merchant country code cannot exceed 3 characters.");

        RuleFor(x => x.MerchantCategoryCode)
            .MaximumLength(4)
            .When(x => !string.IsNullOrEmpty(x.MerchantCategoryCode))
            .WithMessage("Merchant category code cannot exceed 4 characters.");

        RuleFor(x => x.CardHolderName)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.CardHolderName))
            .WithMessage("Card holder name cannot exceed 100 characters.");

        RuleFor(x => x.CardNumberMasked)
            .MaximumLength(19)
            .When(x => !string.IsNullOrEmpty(x.CardNumberMasked))
            .WithMessage("Card number cannot exceed 19 characters.");

        RuleFor(x => x.CardType)
            .MaximumLength(20)
            .When(x => !string.IsNullOrEmpty(x.CardType))
            .WithMessage("Card type cannot exceed 20 characters.");

        RuleFor(x => x.CustomerId)
            .MaximumLength(50)
            .When(x => !string.IsNullOrEmpty(x.CustomerId))
            .WithMessage("Customer ID cannot exceed 50 characters.");

        RuleFor(x => x.AmountLocalCurrency)
            .GreaterThanOrEqualTo(0)
            .When(x => x.AmountLocalCurrency != default)
            .WithMessage("Local currency amount cannot be negative.");

        RuleFor(x => x.LocalCurrency)
            .MaximumLength(3)
            .When(x => !string.IsNullOrEmpty(x.LocalCurrency))
            .WithMessage("Local currency code cannot exceed 3 characters.");

        RuleFor(x => x.ExchangeRate)
            .GreaterThan(0)
            .When(x => x.ExchangeRate != default)
            .WithMessage("Exchange rate must be greater than 0.");

        RuleFor(x => x.PostingDate)
            .GreaterThanOrEqualTo(x => x.Date)
            .When(x => x.PostingDate != default)
            .WithMessage("Posting date cannot be before transaction date.");

        RuleFor(x => x.AuthorizationDate)
            .LessThanOrEqualTo(DateTime.Now)
            .When(x => x.AuthorizationDate != default)
            .WithMessage("Authorization date cannot be in the future.");

        RuleFor(x => x.TerminalId)
            .MaximumLength(20)
            .When(x => !string.IsNullOrEmpty(x.TerminalId))
            .WithMessage("Terminal ID cannot exceed 20 characters.");

        RuleFor(x => x.POSLocation)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.POSLocation))
            .WithMessage("POS location cannot exceed 100 characters.");

        RuleFor(x => x.POSCountryCode)
            .MaximumLength(3)
            .When(x => !string.IsNullOrEmpty(x.POSCountryCode))
            .WithMessage("POS country code cannot exceed 3 characters.");

        RuleFor(x => x.EntryMode)
            .MaximumLength(20)
            .When(x => !string.IsNullOrEmpty(x.EntryMode))
            .WithMessage("Entry mode cannot exceed 20 characters.");

        RuleFor(x => x.AuthorizationCode)
            .MaximumLength(20)
            .When(x => !string.IsNullOrEmpty(x.AuthorizationCode))
            .WithMessage("Authorization code cannot exceed 20 characters.");

        RuleFor(x => x.TransactionType)
            .MaximumLength(20)
            .When(x => !string.IsNullOrEmpty(x.TransactionType))
            .WithMessage("Transaction type cannot exceed 20 characters.");

        RuleFor(x => x.Status)
            .MaximumLength(20)
            .When(x => !string.IsNullOrEmpty(x.Status))
            .WithMessage("Status cannot exceed 20 characters.");

        RuleFor(x => x.ReferenceNumber)
            .MaximumLength(50)
            .When(x => !string.IsNullOrEmpty(x.ReferenceNumber))
            .WithMessage("Reference number cannot exceed 50 characters.");
    }
}