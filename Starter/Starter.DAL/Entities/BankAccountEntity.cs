using System;
using System.Collections.Generic;
using System.Text;

namespace Starter.DAL.Entities
{
    public class BankAccountEntity
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public decimal Balance { get; set; }
        public UserEntity Owner { get; set; }
        public DateTimeOffset OpenedAt { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }
    }
}