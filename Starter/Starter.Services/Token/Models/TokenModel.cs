using System;
using System.Collections.Generic;
using System.Text;

namespace Starter.Services.Token.Models
{
    public class TokenModel
    {
        public string AccessToken { get; set; }

        public DateTimeOffset IssueAt { get; set; }

        public DateTimeOffset ExpiresAt { get; set; }

        public string RefreshToken { get; set; }

        public Guid Id { get; set; }
    }
}