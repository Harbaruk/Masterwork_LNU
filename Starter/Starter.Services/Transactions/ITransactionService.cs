using System;
using System.Collections.Generic;
using System.Text;
using Starter.Services.Transactions.Models;

namespace Starter.Services.Transactions
{
    public interface ITransactionService
    {
        TransactionModel CreateTransaction(CreateTransactionModel model);
        IEnumerable<TransactionModel> GetTransactions(string bankAccountId, TransactionStatus? status);
        TransactionDetailedModel GetTransaction(string id);
    }
}