using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Starter.API.Policies;
using Starter.Common.DomainTaskStatus;
using Starter.Services.BankAccount;
using Starter.Services.BankAccount.Models;
using Starter.Services.Blocks;
using Starter.Services.Blocks.Models;

namespace Starter.API.Controllers
{
    [Produces("application/json")]
    [Route("api/blocks")]
    [Authorize(Policy = AuthPolicies.AuthenticatedUser)]
    public class BlocksController : AbstractController
    {
        private readonly IBlockService _blockService;

        public BlocksController(IBlockService blockService, DomainTaskStatus taskStatus) : base(taskStatus)
        {
            _blockService = blockService;
        }

        [HttpGet]
        [Route("unverified")]
        [ProducesResponseType(typeof(BlockModel), 200)]
        public IActionResult GetUnverifiedBlock()
        {
            return Ok(_blockService.GetUnverifiedBlock());
        }

        [HttpGet]
        [Route("last_verified")]
        [ProducesResponseType(typeof(BlockModel), 200)]
        public IActionResult GetLastVerifiedBlock()
        {
            return Ok(_blockService.GetLastBlock());
        }
    }
}