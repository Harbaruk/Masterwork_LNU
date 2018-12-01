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
        private string _serverHash = "8f00234b-dabb-4765-b616-841d5b92a9a0";
        private CancellationTokenSource _miningCancelationToken;
        private CancellationTokenSource _checkingToken;

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
            _miningCancelationToken = new CancellationTokenSource();
            _checkingToken = new CancellationTokenSource();
        }

        public BlockModel GenerateBlock(IEnumerable<TransactionModel> transactions)
        {
            throw new NotImplementedException();
        }

        public async void Run()
        {
            while (true)
            {
                while (true)
                {
                    Console.WriteLine("Try get transactions");
                    var amount = _serversClient.GetUnverifiedTransactionCount();
                    var localAmount = _unitOfWork.Repository<TransactionEntity>().Set.Count();

                    if (amount + localAmount >= _blockOptions.Value.BlockSize)
                    {
                        break;
                    }
                    Thread.Sleep(10000);
                }

                var transactions = _serversClient.GetUnverifiedTransactions().ToList();
                transactions
                    .AddRange(_unitOfWork.Repository<TransactionEntity>()
                        .Set
                        .Where(x => x.State == TransactionStatus.Pending.ToString())
                        .Select(x => _mapper.Map<TransactionDetailedModel>(x)));
                transactions = transactions.Take(_blockOptions.Value.BlockSize).ToList();

                Console.WriteLine("Start checking task");
                var checkingTask = Task.Factory.StartNew(() =>
                {
                    while (!_miningCancelationToken.IsCancellationRequested)
                    {
                        var block = _serversClient.GetUnverifiedBlock();
                        if (block != null)
                        {
                            _miningCancelationToken.Cancel();
                            return;
                        }
                        Thread.Sleep(10000);
                    }
                });

                Console.WriteLine("Start mining task");
                var miningTask = Task.Factory.StartNew(() =>
                 {
                     var lastBlock = _blockService.GetLastBlock();
                     return Mining(transactions, lastBlock?.Hash);
                 });

                var miningResult = await miningTask;
                if (miningResult.Hash == null)
                {
                    var unverifiedBlock = _blockService.GetUnverifiedBlock();
                    if (CheckBlock(unverifiedBlock))
                    {
                        _blockService.VerifyBlock(unverifiedBlock.Hash);
                    }
                }
                else
                {
                    var block = new CreateBlockModel
                    {
                        Transactions = transactions.Select(x => x.Id).ToList(),
                        Date = DateTimeOffset.Now,
                        Hash = miningResult.Hash,
                        Miner = _serverHash,
                        Nonce = miningResult.Nonce,
                        PrevBlockHash = _blockService.GetLastBlock()?.Hash ?? "000000000000000"
                    };
                    _blockService.CreateBlock(block);
                }
                _miningCancelationToken = new CancellationTokenSource();
            }
        }

        private bool CheckBlock(UnverifiedBlockModel unverifiedBlock)
        {
            var blockTemplate = new BlockTemplateModel
            {
                PrevHash = unverifiedBlock.PrevHash,
                Miner = unverifiedBlock.Miner,
                Nonce = unverifiedBlock.Nonce,
                TransactionsMerkleTree = MerkleTree<TransactionDetailedModel>.Compute(unverifiedBlock.Transactions)
            };
            using (SHA256Managed sha = new SHA256Managed())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Position = 0;
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(ms, blockTemplate);

                    var blockResultHash = Convert.ToBase64String(sha.ComputeHash(ms.ToArray()));
                    Console.WriteLine(blockResultHash);
                    return blockResultHash == unverifiedBlock.Hash;
                }
            }
        }

        private (long Nonce, string Hash) Mining(List<TransactionDetailedModel> transactions, string prevHash)
        {
            var transactionHash = MerkleTree<TransactionDetailedModel>.Compute(transactions);
            string blockResultHash;

            var blockTemplate = new BlockTemplateModel
            {
                PrevHash = prevHash,
                Miner = _serverHash,
                Nonce = 0,
                TransactionsMerkleTree = transactionHash
            };
            var analyticsTask = Task.Factory.StartNew(() => { while (!_miningCancelationToken.IsCancellationRequested) { Console.WriteLine(blockTemplate.Nonce); Thread.Sleep(10000); } });
            using (SHA256Managed sha = new SHA256Managed())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    do
                    {
                        if (_miningCancelationToken.IsCancellationRequested)
                        {
                            return (0, null);
                        }
                        blockTemplate.Nonce++;
                        ms.Position = 0;
                        BinaryFormatter bf = new BinaryFormatter();
                        bf.Serialize(ms, blockTemplate);

                        blockResultHash = Convert.ToBase64String(sha.ComputeHash(ms.ToArray()));
                        Console.WriteLine(blockResultHash);
                    }
                    while (!blockResultHash.Substring(0, 3).All(x => x == '0'));
                    _miningCancelationToken.Cancel();
                    return (blockTemplate.Nonce, blockResultHash);
                }
            }
        }
    }
}