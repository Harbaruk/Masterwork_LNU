﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Starter.Services.Transactions.Models
{
    [Serializable]
    public class TransactionDetailedModel
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string FromAccount { get; set; }
        public string ToAccount { get; set; }
        public DateTimeOffset SentTime { get; set; }
        public DateTimeOffset? ProcessedTime { get; set; }
        public decimal Money { get; set; }
        public TransactionStatus Status { get; set; }
        public string BlockId { get; set; }
    }
}