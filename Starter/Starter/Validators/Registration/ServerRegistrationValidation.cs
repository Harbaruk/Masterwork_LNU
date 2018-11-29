using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Starter.Services.Registration.Models;

namespace Starter.API.Validators.Registration
{
    public class ServerRegistrationValidation : AbstractValidator<ServerRegistrationModel>
    {
        public ServerRegistrationValidation()
        {
            RuleFor(x => x.Password)
                .MinimumLength(8)
                .WithMessage("password is too short");
        }
    }
}