using System;
using System.Collections.Generic;
using System.Text;

namespace Starter.DAL.Entities
{
    public class TwoFactorAuthEntity
    {
        public int Id { get; set; }

        public string Secret { get; set; }

        public UserEntity User { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}