using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Starter.API.Policies;
using Starter.Common.DomainTaskStatus;
using Starter.Services.Token;
using Starter.Services.Token.Models;
using Starter.Services.TwoFactorAuth.Models;

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

        [HttpPost]
        [Route("auth/verify")]
        [ProducesResponseType(typeof(TokenModel), 200)]
        [Authorize(Policy = AuthPolicies.TwoFactorAuth)]
        public IActionResult Verify([FromBody] TwoFactorAuthModel authModel)
        {
            return Ok(_tokenService.GetToken(authModel));
        }
    }
}