using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Starter.Services.Token.Models;

namespace Starter.API.Validators.Token
{
    public class LoginCredentialsValidator : AbstractValidator<LoginCredentials>
    {
        public LoginCredentialsValidator()
        {
            RuleFor(x => x.GrantType)
                .Equal("password")
                .WithMessage("Invalid grant type");

            RuleFor(x => x.Password)
                .MinimumLength(8)
                .WithMessage("password is too short");
        }
    }
}