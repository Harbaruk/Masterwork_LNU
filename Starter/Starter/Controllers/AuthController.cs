using Microsoft.AspNetCore.Mvc;
using Starter.Common.DomainTaskStatus;
using Starter.Services.Token;
using Starter.Services.Token.Models;

namespace Starter.API.Controllers
{
    [Produces("application/json")]
    [Route("api")]
    public class AuthController : AbstractController
    {
        private readonly ITokenService _tokenService;

        public AuthController(ITokenService tokenService, DomainTaskStatus taskStatus) : base(taskStatus)
        {
            _tokenService = tokenService;
        }

        [HttpPost]
        [Route("token")]
        [ProducesResponseType(typeof(TokenModel), 200)]
        public IActionResult AccessToken([FromBody] LoginCredentials loginCredentials)
        {
            return Ok(_tokenService.GetToken(loginCredentials));
        }

        [HttpPost]
        [Route("refresh_token")]
        [ProducesResponseType(typeof(TokenModel), 200)]
        public IActionResult RefreshToken([FromBody] RefreshTokenModel refreshToken)
        {
            return Ok(_tokenService.GetRefreshToken(refreshToken));
        }
    }
}