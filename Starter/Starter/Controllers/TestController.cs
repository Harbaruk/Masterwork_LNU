using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Starter.Services.TestDataGenerationService;
using Starter.Services.TestDataGenerationService.Models;

namespace Starter.API.Controllers
{
    [Produces("application/json")]
    [Route("api/test")]
    public class TestController : Controller
    {
        private readonly ITestDataGenerationService _generationService;

        public TestController(ITestDataGenerationService generationService)
        {
            _generationService = generationService;
        }

        [HttpGet]
        [Route("create_user")]
        [ProducesResponseType(typeof(TestUserModel), 200)]
        public IActionResult CreateTestUser()
        {
            return Ok(_generationService.CreateTestUser());
        }

        [HttpGet]
        [Route("generate_transactions")]
        public IActionResult GenerateTransactions(string fromAccount = null, string toAccount = null, int number = 200)
        {
            _generationService.GenerateTransactions(fromAccount, toAccount, number);
            return Ok();
        }
    }
}