using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Starter.Services.Token.Models;

namespace Starter.API.Validators.Token
{
    public class RefreshTokenValidator : AbstractValidator<RefreshTokenModel>
    {
        public RefreshTokenValidator()
        {
            RuleFor(x => x.GrantType)
                .Equal("refresh_token")
                .WithMessage("Invalid grant type");
        }
    }
}