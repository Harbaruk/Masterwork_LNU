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
        private CancellationTokenSource _txCheckToken;

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
            _txCheckToken = new CancellationTokenSource();
        }

        public BlockModel GenerateBlock(IEnumerable<TransactionModel> transactions)
        {
            throw new NotImplementedException();
        }

        public async void Run()
        {
            while (true)
            {
                var t = Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        Console.WriteLine("Try get transactions");
                        var amount = _serversClient.GetUnverifiedTransactionCount();
                        Thread.Sleep(100);
                        var localAmount = _unitOfWork.Repository<TransactionEntity>().Set.Count();

                        if (amount + localAmount >= _blockOptions.Value.BlockSize)
                        {
                            break;
                        }
                        Thread.Sleep(10000);
                    }
                }).ContinueWith((res) =>
                {
                    Console.WriteLine("Start mining");
                    var transactions = _serversClient.GetUnverifiedTransactions().ToList();
                    transactions
                        .AddRange(_unitOfWork.Repository<TransactionEntity>()
                            .Set
                            .Where(x => x.State == TransactionStatus.Pending.ToString())
                            .Select(x => _mapper.Map<TransactionDetailedModel>(x)));
                    transactions = transactions.Take(_blockOptions.Value.BlockSize).OrderBy(x => x.SentTime).ToList();

                    var lastBlock = _blockService.GetLastBlock();
                    return (Transaction: transactions, Mining: Mining(transactions, lastBlock?.Hash));
                });
                Console.WriteLine("Start checking task");
                var checkingTask = Task.Factory.StartNew(() =>
                {
                    while (!_checkingToken.IsCancellationRequested)
                    {
                        Thread.Sleep(500);
                        var block = _serversClient.GetUnverifiedBlock();
                        if (block != null)
                        {
                            _miningCancelationToken.Cancel();
                            _txCheckToken.Cancel();
                            _checkingToken.Cancel();
                        }
                        Thread.Sleep(10000);
                    }
                });

                Task.WaitAny(t, checkingTask);

                if (t.Result.Mining.Hash == null)
                {
                    var unverifiedBlock = _blockService.GetUnverifiedBlock();
                    if (CheckBlock(unverifiedBlock))
                    {
                        _blockService.VerifyBlock(unverifiedBlock.BlockHash, _serverHash);
                    }
                }
                else
                {
                    var block = new CreateBlockModel
                    {
                        Transactions = t.Result.Transaction.Select(x => x.Id).ToList(),
                        Date = DateTimeOffset.Now,
                        Hash = t.Result.Mining.Hash.TrimEnd('='),
                        Miner = _serverHash,
                        Nonce = t.Result.Mining.Nonce,
                        PrevBlockHash = _blockService.GetLastBlock()?.Hash.TrimEnd('=') ?? "000000000000000"
                    };
                    _blockService.CreateBlock(block);
                }
                _miningCancelationToken = new CancellationTokenSource();
                _txCheckToken = new CancellationTokenSource();
                _checkingToken = new CancellationTokenSource();
            }
        }

        private bool CheckBlock(UnverifiedBlockModel unverifiedBlock)
        {
            foreach (var t in unverifiedBlock.Transactions)
            {
                t.BlockId = null;
                t.ProcessedTime = null;
            }
            var blockTemplate = new BlockTemplateModel
            {
                PrevHash = unverifiedBlock.PrevHash.TrimEnd('='),
                Miner = unverifiedBlock.Miner,
                Nonce = unverifiedBlock.Nonce,
                TransactionsMerkleTree = MerkleTree<TransactionDetailedModel>.Compute(unverifiedBlock.Transactions).TrimEnd('=')
            };
            using (SHA256Managed sha = new SHA256Managed())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Position = 0;
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(ms, blockTemplate);

                    var blockResultHash = Convert.ToBase64String(sha.ComputeHash(ms.ToArray())).TrimEnd('=');
                    Console.WriteLine(blockResultHash);
                    return blockResultHash == unverifiedBlock.BlockHash;
                }
            }
        }

        private (long Nonce, string Hash) Mining(List<TransactionDetailedModel> transactions, string prevHash)
        {
            var transactionHash = MerkleTree<TransactionDetailedModel>.Compute(transactions).TrimEnd('=');
            string blockResultHash;

            var blockTemplate = new BlockTemplateModel
            {
                PrevHash = prevHash?.TrimEnd('=') ?? "000000000000000",
                Miner = _serverHash,
                Nonce = 0,
                TransactionsMerkleTree = transactionHash
            };
            var analyticsTask = Task.Factory.StartNew(() =>
            {
                while (!_miningCancelationToken.IsCancellationRequested)
                {
                    Console.WriteLine(blockTemplate.Nonce);
                    Thread.Sleep(10000);
                }
            });
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
                    }
                    while (!blockResultHash.Substring(0, 3).All(x => x == '0'));
                    _miningCancelationToken.Cancel();
                    return (blockTemplate.Nonce, blockResultHash);
                }
            }
        }
    }
}