using System;
using System.Collections.Generic;
using System.Text;

namespace Starter.DAL.Entities
{
    public class BankAccountEntity
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public decimal Balance { get; set; }
        public UserEntity Owner { get; set; }
        public DateTimeOffset OpenedAt { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }
        public IEnumerable<TransactionEntity> SentTransactions { get; set; }
        public IEnumerable<TransactionEntity> ReceivedTransactions { get; set; }
    }
}