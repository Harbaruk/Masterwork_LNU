using System;
using System.Collections.Generic;
using System.Text;

namespace Starter.Services.Mining
{
    [Serializable]
    public class BlockTemplateModel
    {
        public string PrevHash { get; set; }
        public long Nonce { get; set; }
        public string TransactionsMerkleTree { get; set; }
        public string Miner { get; set; }
    }
}