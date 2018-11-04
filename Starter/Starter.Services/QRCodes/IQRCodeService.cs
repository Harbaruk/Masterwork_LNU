using Starter.Services.QRCodes.Models;

namespace Starter.Services.QRCodes
{
    public interface IQRCodeService
    {
        string GenerateQR(TotpEncodingOptions content);
    }
}