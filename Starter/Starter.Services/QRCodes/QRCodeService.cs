using System.Text;
using Microsoft.Extensions.Options;
using QRCoder;
using QrCodeGenerator = QRCoder.QRCodeGenerator;
using Starter.Services.TwoFactorAuth.TOTP;
using Starter.Services.QRCodes.Models;

namespace Starter.Services.QRCodes
{
    public class QRCodeService : IQRCodeService
    {
        private readonly IOptions<TotpOptions> _totpOptions;
        private readonly ITotpProvider _totpProvider;

        public QRCodeService(IOptions<TotpOptions> totpOptions, ITotpProvider totpProvider)
        {
            _totpOptions = totpOptions;
            _totpProvider = totpProvider;
        }

        public string GenerateQR(TotpEncodingOptions content)
        {
            StringBuilder builder = new StringBuilder(_totpOptions.Value.Protocol);
            builder = builder
                .Replace("{issuer}", content.Issuer)
                .Replace("{email}", content.Email)
                .Replace("{secret}", content.Secret);

            return GenerateQR(builder.ToString());
        }

        private string GenerateQR(string data)
        {
            var qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            Base64QRCode qrCode = new Base64QRCode(qrCodeData);

            return $"data:image/png;base64,{qrCode.GetGraphic(10)}";
        }
    }
}