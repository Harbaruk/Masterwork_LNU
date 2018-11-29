using System;
using System.Collections.Generic;
using System.Text;
using Starter.Services.Registration.Models;

namespace Starter.Services.Registration
{
    public interface IRegistrationService
    {
        FinishRegistrationModel Register(UserRegistrationModel user);
        void RegisterServer(ServerRegistrationModel user);
    }
}