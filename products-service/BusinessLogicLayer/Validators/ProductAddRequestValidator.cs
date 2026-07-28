using BusinessLogicLayer.DTO;
using FluentValidation;

namespace BusinessLogicLayer.Validators;

public class ProductAddRequestValidator : AbstractValidator<ProductAddRequest>
{
    public ProductAddRequestValidator()
    {
        //ProductName
        RuleFor(temp => temp.ProductName).NotEmpty()
            .WithMessage("{PropertyName} is required");

        //Category
        RuleFor(temp => temp.Category).IsInEnum()
            .WithMessage("{PropertyName} is not a valid enum value");

        //UnitPrice
        RuleFor(temp => temp.UnitPrice).InclusiveBetween(0, double.MaxValue)
            .WithMessage("{PropertyName} must be between 0 and {To}");

        //QuantityInStock
        RuleFor(temp => temp.QuantityInStock).InclusiveBetween(0, int.MaxValue)
            .WithMessage("{PropertyName} must be between 0 and {To}");
    }
}
