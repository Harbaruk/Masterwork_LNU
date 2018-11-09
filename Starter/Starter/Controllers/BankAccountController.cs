using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Starter.API.Policies;
using Starter.Services.BankAccount;
using Starter.Services.BankAccount.Models;

namespace Starter.API.Controllers
{
    [Produces("application/json")]
    [Route("api/bank_account")]
    [Authorize(Policy = AuthPolicies.AuthenticatedUser)]
    public class BankAccountController : Controller
    {
        private readonly IBankAccountService _bankAccountService;

        public BankAccountController(IBankAccountService bankAccountService)
        {
            _bankAccountService = bankAccountService;
        }

        [HttpPost]
        [Route("create")]
        [ProducesResponseType(typeof(BankAccountModel), 200)]
        public IActionResult CreateAccount([FromBody] CreateBankAccountModel accountModel)
        {
            return Ok(_bankAccountService.CreateBankAccount(accountModel));
        }

        [HttpGet]
        [Route("get")]
        [ProducesResponseType(typeof(List<BankAccountModel>), 200)]
        public IActionResult GetAccounts(BankAccountType? type = null, BankAccountStatus? status = null)
        {
            return Ok(_bankAccountService.GetAccounts(type, status));
        }
    }
}