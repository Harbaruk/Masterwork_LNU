using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Starter.Common.DomainTaskStatus;
using Starter.Services.Registration;
using Starter.Services.Registration.Models;
using Starter.Services.Token.Models;
using Starter.Services.TwoFactorAuth.Models;

namespace Starter.API.Controllers
{
    [Produces("application/json")]
    [Route("api/registration")]
    public class RegistrationController : AbstractController
    {
        private readonly IRegistrationService _registrationService;

        public RegistrationController(IRegistrationService registrationService, DomainTaskStatus taskStatus) : base(taskStatus)
        {
            _registrationService = registrationService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(FinishRegistrationModel), 200)]
        public IActionResult Register([FromBody] UserRegistrationModel user)
        {
            return Ok(_registrationService.Register(user));
        }
    }
}