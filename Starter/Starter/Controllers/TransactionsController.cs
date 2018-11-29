using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Starter.API.Policies;
using Starter.Common.DomainTaskStatus;
using Starter.Services.Transactions;
using Starter.Services.Transactions.Models;

namespace Starter.API.Controllers
{
    [Produces("application/json")]
    [Route("api/transactions")]
    [Authorize(Policy = AuthPolicies.AuthenticatedUser)]
    public class TransactionsController : AbstractController
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService, DomainTaskStatus taskStatus) : base(taskStatus)
        {
            _transactionService = transactionService;
        }

        [HttpPost]
        [Route("create")]
        [ProducesResponseType(typeof(TransactionModel), 200)]
        public IActionResult CreateTransaction([FromBody] CreateTransactionModel transactionModel)
        {
            return Ok(_transactionService.CreateTransaction(transactionModel));
        }

        [HttpGet]
        [ProducesResponseType(typeof(TransactionDetailedModel), 200)]
        public IActionResult GetById(string id)
        {
            return Ok(_transactionService.GetTransaction(id));
        }

        [HttpGet]
        [Route("filter")]
        [ProducesResponseType(typeof(IEnumerable<TransactionModel>), 200)]
        public IActionResult GetTransactions(string id, TransactionStatus? status)
        {
            return Ok(_transactionService.GetTransactions(id, status));
        }

        [HttpGet]
        [Route("unverified")]
        [ProducesResponseType(typeof(IEnumerable<TransactionDetailedModel>), 200)]
        public IActionResult GetUnverified()
        {
            return Ok(_transactionService.GetUnverifiedTransactions());
        }

        [HttpGet]
        [Route("unverified_count")]
        [ProducesResponseType(typeof(int), 200)]
        public IActionResult GetUnverifiedTransactionsCount()
        {
            return Ok(_transactionService.CountUnverifiedTransactions());
        }
    }
}