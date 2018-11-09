using System;
using System.Collections.Generic;
using System.Text;

namespace Starter.Services.Transactions.Models
{
    public class TransactionModel
    {
        public string Id { get; set; }
        public decimal Balance { get; set; }
        public DateTimeOffset Date { get; set; }
        public TransactionStatus Status { get; set; }
    }
}