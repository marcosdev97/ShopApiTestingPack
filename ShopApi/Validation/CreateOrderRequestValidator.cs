using FluentValidation;
using ShopApi.Contracts;

namespace ShopApi.Validation;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.CustomerEmail)
            .NotEmpty().WithMessage("CustomerEmail is required")
            .EmailAddress().WithMessage("CustomerEmail must be a valid email");

        RuleFor(x => x.Lines)
            .NotNull().WithMessage("Lines are required")
            .Must(x => x is { Count: > 0 }).WithMessage("At least one line is required");

        RuleForEach(x => x.Lines).ChildRules(line =>
        {
            line.RuleFor(l => l.ProductId).NotEmpty().WithMessage("ProductId is required");
            line.RuleFor(l => l.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0");
        });
    }
}
