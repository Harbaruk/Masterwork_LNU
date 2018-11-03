using System;
using System.Collections.Generic;
using System.Text;

namespace Starter.Services.Token.Models
{
    public class RefreshTokenModel
    {
        public string RefreshToken { get; set; }
        public string GrantType { get; set; }
    }
}