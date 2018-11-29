using System;
using System.Collections.Generic;
using System.Text;
using Starter.Services.Blocks.Models;
using Starter.Services.Transactions.Models;

namespace Starter.Services.Mining.ServersClient
{
    public interface IServersClient
    {
        int GetUnverifiedTransactionCount();
        CreateBlockModel GetUnverifiedBlock();
        string GetLastBlock();
        void VerifyBlock(string blockId);
        IEnumerable<TransactionDetailedModel> GetUnverifiedTransactions();
    }
}