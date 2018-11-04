using System;
using System.Collections.Generic;
using System.Text;

namespace Starter.Services.QRCodes.Models
{
    public class TotpEncodingOptions
    {
        public string Issuer { get; set; }

        public string Email { get; set; }

        public string Secret { get; set; }
    }
}