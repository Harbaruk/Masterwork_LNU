using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Starter.Common.DomainTaskStatus;

namespace Starter.API.Controllers
{
    [Produces("application/json")]
    [Route("api/registration")]
    public class RegistrationController : AbstractController
    {
        public RegistrationController(DomainTaskStatus taskStatus) : base(taskStatus)
        {
        }
    }
}