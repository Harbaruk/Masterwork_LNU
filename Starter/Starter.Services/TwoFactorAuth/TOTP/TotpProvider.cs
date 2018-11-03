using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using OtpNet;
using Starter.Common.TimeAPI;
using Starter.DAL.Infrastructure;

namespace Starter.Services.TwoFactorAuth.TOTP
{
    public class TotpProvider : ITotpProvider
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptions<TotpOptions> _options;
        private readonly TimeAPI _timeAPI;

        public TotpProvider(IUnitOfWork unitOfWork, IOptions<TotpOptions> options, TimeAPI timeAPI)
        {
            _unitOfWork = unitOfWork;
            _options = options;
            _timeAPI = timeAPI;
        }

        public string ToBase32(string secret)
        {
            return Base32Encoding.ToString(Encoding.ASCII.GetBytes(secret)).TrimEnd('=');
        }

        public bool Verify(string secret, string code)
        {
            var correctTime = _timeAPI.GetGoogleTime();
            var otp = new Totp(Base32Encoding.ToBytes(secret), mode: OtpHashMode.Sha1, timeCorrection: new TimeCorrection(correctTime));

            return otp.VerifyTotp(code, out long time);
        }
    }
}