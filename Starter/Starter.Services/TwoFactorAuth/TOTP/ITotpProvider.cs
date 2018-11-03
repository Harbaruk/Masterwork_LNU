using System;
using System.Collections.Generic;
using System.Text;

namespace Starter.Services.TwoFactorAuth.TOTP
{
    public interface ITotpProvider
    {
        string ToBase32(string secret);

        bool Verify(string secret, string code);
    }
}