using FluentAssertions;
using ShopApi.Contracts;
using ShopApi.Validation;
using Xunit;

namespace ShopApi.Tests.Unit;

public class ValidatorsTests
{
    [Fact]
    public void CreateProductRequestValidator_should_fail_when_invalid()
    {
        var validator = new CreateProductRequestValidator();

        var result = validator.Validate(new CreateProductRequest("", 0m));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
        result.Errors.Should().Contain(e => e.PropertyName == "Price");
    }

    [Fact]
    public void CreateOrderRequestValidator_should_fail_when_no_lines()
    {
        var validator = new CreateOrderRequestValidator();

        var result = validator.Validate(new CreateOrderRequest("buyer@test.com", new List<CreateOrderLineRequest>()));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("At least one line"));
    }
}
