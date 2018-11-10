using System;
using System.Collections.Generic;
using System.Text;

namespace Starter.Services.Transactions.Models
{
    public class CreateTransactionModel
    {
        public decimal Money { get; set; }
        public string ToAccount { get; set; }
        public string Desctiption { get; set; }
        public string FromAccount { get; set; }
    }
}