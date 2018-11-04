using System;
using System.Collections.Generic;
using System.Text;
using Starter.Services.Token.Models;

namespace Starter.Services.Registration.Models
{
    public class FinishRegistrationModel
    {
        public TokenModel Token { get; set; }
        public string QRCode { get; set; }
    }
}