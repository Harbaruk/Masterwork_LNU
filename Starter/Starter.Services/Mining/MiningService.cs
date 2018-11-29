using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using Starter.API.Policies;
using Starter.DAL.Entities;
using Starter.DAL.Infrastructure;
using Starter.Services.Blocks;
using Starter.Services.Blocks.Models;
using Starter.Services.Mining.ServersClient;
using Starter.Services.Transactions;
using Starter.Services.Transactions.Models;

namespace Starter.Services.Mining
{
    public class MiningService : IMiningService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransactionService _transactionService;
        private readonly IBlockService _blockService;
        private readonly IOptions<TrustfullServersOptions> _options;
        private readonly IOptions<BlockSettingsOptions> _blockOptions;
        private readonly IMapper _mapper;
        private readonly IServersClient _serversClient;
        private readonly CancellationToken _miningCancelationToken;

        public MiningService(IUnitOfWork unitOfWork,
            ITransactionService transactionService,
            IBlockService blockService,
            IOptions<TrustfullServersOptions> serverOptions,
            IOptions<BlockSettingsOptions> blockOptions,
            IMapper mapper,
            IServersClient serversClient)
        {
            _unitOfWork = unitOfWork;
            _transactionService = transactionService;
            _blockService = blockService;
            _options = serverOptions;
            _blockOptions = blockOptions;
            _mapper = mapper;
            _serversClient = serversClient;
            _miningCancelationToken = new CancellationToken();
        }

        public BlockModel GenerateBlock(IEnumerable<TransactionModel> transactions)
        {
            throw new NotImplementedException();
        }

        public async void Run()
        {
            while (true)
            {
                var task = Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        var amount = _serversClient.GetUnverifiedTransactionCount();
                        var localAmount = _unitOfWork.Repository<TransactionEntity>().Set.Count();

                        if (amount + localAmount >= _blockOptions.Value.BlockSize)
                        {
                            break;
                        }
                        Thread.Sleep(10000);
                    }
                });

                Task.WaitAll(task);

                var transactions = _serversClient.GetUnverifiedTransactions().ToList();
                transactions
                    .AddRange(_unitOfWork.Repository<TransactionEntity>()
                        .Set
                        .Where(x => x.State == TransactionStatus.Pending.ToString())
                        .Select(x => _mapper.Map<TransactionDetailedModel>(x)));
                transactions = transactions.Take(_blockOptions.Value.BlockSize).ToList();

                var checkingTask = Task.Factory.StartNew(() =>
                {
                    _serversClient.GetUnverifiedBlock();
                    Thread.Sleep(10000);
                }
                );
            }
        }

        private (long Nonce, string Hash) Mining(List<TransactionDetailedModel> transactions, string prevHash)
        {
            var transactionHash = MerkleTree<TransactionDetailedModel>.Compute(transactions);
            string blockResultHash;
            var blockTemplate = new BlockTemplateModel
            {
                PrevHash = prevHash,
                Miner = "Miner",
                Nonce = 0,
                TransactionsMerkleTree = transactionHash
            };

            using (SHA256Managed sha = new SHA256Managed())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    do
                    {
                        blockTemplate.Nonce++;
                        ms.Position = 0;
                        BinaryFormatter bf = new BinaryFormatter();
                        bf.Serialize(ms, blockTemplate);

                        blockResultHash = Convert.ToBase64String(sha.ComputeHash(ms.ToArray()));
                        Console.WriteLine(blockResultHash);
                    }
                    while (!blockResultHash.Substring(0, 5).All(x => x == '0'));
                    return (blockTemplate.Nonce, blockResultHash);
                }
            }
        }
    }
}