using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.Options;
using Starter.Common.DomainTaskStatus;
using Starter.DAL.Entities;
using Starter.DAL.Infrastructure;
using Starter.Services.Blocks.Models;
using Starter.Services.Enums;
using Starter.Services.Providers;
using Starter.Services.Transactions.Models;

namespace Starter.Services.Blocks
{
    public class BlockService : IBlockService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenticatedUser _authenticatedUser;
        private readonly IOptions<BlockSettingsOptions> _options;
        private readonly DomainTaskStatus _taskStatus;
        private readonly IMapper _mapper;

        public BlockService(
            IUnitOfWork unitOfWork,
            IAuthenticatedUser authenticatedUser,
            IOptions<BlockSettingsOptions> options,
            DomainTaskStatus taskStatus,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _authenticatedUser = authenticatedUser;
            _options = options;
            _taskStatus = taskStatus;
            _mapper = mapper;
        }

        public BlockModel CreateBlock(CreateBlockModel model)
        {
            var blocRepo = _unitOfWork.Repository<BlockEntity>();

            var prevBlock = blocRepo.Set.FirstOrDefault(x => x.Hash == model.PrevBlockHash);

            if (prevBlock == null && blocRepo.Set.Count() != 0)
            {
                _taskStatus.AddUnkeyedError("invalid prev block hash");
                return null;
            }
            var miner = _unitOfWork.Repository<TrustfullServerEntity>().Set
                .FirstOrDefault(x => x.Hash.ToString() == model.Miner);

            if (miner == null)
            {
                _taskStatus.AddUnkeyedError("invalid user id");
                return null;
            }

            var transactions = _unitOfWork
                .Repository<TransactionEntity>()
                .Set
                .Where(x => model.Transactions.Contains(x.Id));

            if (transactions.Count() != _options.Value.BlockSize)
            {
                _taskStatus.AddUnkeyedError("invalid block size");
                return null;
            }

            var blockEntity = new BlockEntity
            {
                BlockState = BlockStatus.Pending.ToString(),
                Date = DateTimeOffset.Now,
                Hash = model.Hash,
                Miner = miner,
                Nonce = model.Nonce,
                PreviousBlockHash = model.PrevBlockHash,
                Transactions = transactions.ToList(),
            };

            blocRepo.Insert(blockEntity);
            _unitOfWork.SaveChanges();

            return new BlockModel
            {
                Hash = blockEntity.Hash,
                Date = blockEntity.Date,
                Miner = miner.PublicKey.ToString()
            };
        }

        public IEnumerable<BlockModel> GetBlocks(int take, int skip)
        {
            return _unitOfWork.Repository<BlockEntity>()
                .Include(x => x.Miner)
                .Select(x => new BlockModel
                {
                    Date = x.Date,
                    Hash = x.Hash,
                    Miner = x.Miner.PublicKey.ToString()
                })
                .Skip(skip)
                .Take(take);
        }

        public BlockModel GetLastBlock()
        {
            var locker = new Object();
            lock (locker)
            {
                return _mapper.Map<BlockModel>(_unitOfWork.Repository<BlockEntity>().Set.Where(x => x.BlockState == BlockStatus.Accepted.ToString())
                    .OrderByDescending(x => x.Date)
                    .FirstOrDefault());
            }
        }

        public UnverifiedBlockModel GetUnverifiedBlock()
        {
            return _mapper.Map<UnverifiedBlockModel>(_unitOfWork.Repository<BlockEntity>()
                .Set
                .FirstOrDefault(x => x.BlockState == BlockStatus.Pending.ToString()));
        }

        public void SaveVerifiedBlock(UnverifiedBlockModel model)
        {
            var transactions = _unitOfWork.Repository<TransactionEntity>()
                .Include(x => x.Block)
                .Where(x => model.Transactions.Select(t => t.Id).Contains(x.Id));

            var block = _unitOfWork.Repository<BlockEntity>().Set.FirstOrDefault(x => x.Id == model.Id);
            block.Miner = _unitOfWork.Repository<TrustfullServerEntity>().Set.FirstOrDefault(x => x.PublicKey == model.Miner);
            foreach (var t in transactions)
            {
                t.State = TransactionStatus.Accepted.ToString();
                t.Block = block;
            }
            _unitOfWork.SaveChanges();
        }

        public void VerifyBlock(string blockId)
        {
            if (_authenticatedUser.Role != UserRoles.Server)
            {
                _taskStatus.AddUnkeyedError("not allowed");
                return;
            }

            var block = _unitOfWork.Repository<BlockEntity>().Include(x => x.Verifications, x => x.Transactions).FirstOrDefault(x => x.Hash == blockId);
            var isAlreadyVerify = block.Verifications.Any(x => x.UserPublicKey == _authenticatedUser.ServerHash);

            if (!isAlreadyVerify)
            {
                var verification = new BlockVerificationEntity
                {
                    Block = block,
                    UserPublicKey = _authenticatedUser.ServerHash
                };

                _unitOfWork.Repository<BlockVerificationEntity>().Insert(verification);
                _unitOfWork.SaveChanges();

                if (block.Verifications.Count >= _options.Value.BlockVerificationAmount)
                {
                    block.BlockState = BlockStatus.Accepted.ToString();
                    var transactionsToUpdate = _unitOfWork.Repository<TransactionEntity>().Include(x => x.FromAccount, x => x.ToAccount).Where(x => block.Transactions.Select(y => y.Id).Contains(x.Id));
                    foreach (var tx in transactionsToUpdate)
                    {
                        tx.State = TransactionStatus.Accepted.ToString();
                        tx.FromAccount.Balance -= tx.Money;
                        tx.ToAccount.Balance += tx.Money;
                    }
                    _unitOfWork.SaveChanges();
                }
            }
        }
    }
}