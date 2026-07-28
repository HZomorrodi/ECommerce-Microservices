using eCommerce.Core.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.Core.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        //Email
        RuleFor(temp => temp.Email).NotEmpty().WithMessage("{PropertyName} is required")
            .EmailAddress().WithMessage("Invalid Email Address format");

        //Password
        RuleFor(temp => temp.Password).NotEmpty().WithMessage("{PropertyName} is required");
    }
}
