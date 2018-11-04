using System;
using Starter.Services.Token.Models;
using Starter.Services.TwoFactorAuth.Models;

namespace Starter.Services.Token
{
    public interface ITokenService
    {
        TokenModel GetToken(LoginCredentials loginCredentials);

        TokenModel GetRefreshToken(RefreshTokenModel refreshToken);

        TokenModel GetToken(TwoFactorAuthModel twoFactor);

        TokenModel GetRegistrationToken(string userEmail);
    }
}