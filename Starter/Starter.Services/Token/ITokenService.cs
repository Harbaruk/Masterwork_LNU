using System;
using Starter.Services.Token.Models;

namespace Starter.Services.Token
{
    public interface ITokenService
    {
        TokenModel GetToken(LoginCredentials loginCredentials);

        TokenModel GetRefreshToken(RefreshTokenModel refreshToken);
    }
}