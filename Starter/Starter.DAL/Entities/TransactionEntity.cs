using System;
using System.Collections.Generic;
using System.Text;

namespace Starter.DAL.Entities
{
    public class TransactionEntity
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public DateTimeOffset Date { get; set; }

        public string State { get; set; }

        public BankAccountEntity FromAccount { get; set; }
        public BankAccountEntity ToAccount { get; set; }
        public UserEntity Initiator { get; set; }

        public BlockEntity Block { get; set; }
    }
}