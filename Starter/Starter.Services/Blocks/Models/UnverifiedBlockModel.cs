using System;
using System.Collections.Generic;
using System.Text;
using Starter.Services.Transactions.Models;

namespace Starter.Services.Blocks.Models
{
    public class UnverifiedBlockModel
    {
        public int Id { get; set; }
        public string Hash { get; set; }
        public string PrevHash { get; set; }
        public List<TransactionDetailedModel> Transactions { get; set; }
        public string Miner { get; set; }
        public long Nonce { get; set; }
    }
}