using System;
using System.Collections.Generic;
using System.Text;
using Starter.Services.Transactions.Models;

namespace Starter.Services.BankAccount.Models
{
    public class BankAccountDetailedModel
    {
        public string Id { get; set; }
        public BankAccountType Type { get; set; }
        public decimal Balance { get; set; }
        public IEnumerable<TransactionModel> Transactions { get; set; }
        public DateTimeOffset OpenedAt { get; set; }
        public DateTimeOffset? ClosedAt { get; set; }
    }
}