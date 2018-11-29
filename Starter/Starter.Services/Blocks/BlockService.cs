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

            if (prevBlock == null)
            {
                _taskStatus.AddUnkeyedError("invalid prev block hash");
                return null;
            }
            var miner = _unitOfWork.Repository<UserEntity>().Set
                .FirstOrDefault(x => x.Id.ToString() == model.Miner && _authenticatedUser.Id.ToString() == model.Miner);

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
                PreviousBlockHash = prevBlock.Hash,
                Transactions = transactions.ToList(),
            };

            blocRepo.Insert(blockEntity);
            _unitOfWork.SaveChanges();

            return new BlockModel
            {
                Id = blockEntity.Hash,
                Date = blockEntity.Date,
                Miner = miner.Id.ToString()
            };
        }

        public IEnumerable<BlockModel> GetBlocks(int take, int skip)
        {
            return _unitOfWork.Repository<BlockEntity>()
                .Include(x => x.Miner)
                .Select(x => new BlockModel
                {
                    Date = x.Date,
                    Id = x.Hash,
                    Miner = x.Miner.Id.ToString()
                })
                .Skip(skip)
                .Take(take);
        }

        public BlockModel GetLastBlock()
        {
            return _mapper.Map<BlockModel>(_unitOfWork.Repository<BlockEntity>().Set.Where(x => x.BlockState == BlockStatus.Accepted.ToString())
                .OrderByDescending(x => x.Date)
                .FirstOrDefault());
        }

        public BlockModel GetUnverifiedBlock()
        {
            return _mapper.Map<BlockModel>(_unitOfWork.Repository<BlockEntity>()
                .Set
                .FirstOrDefault(x => x.BlockState == BlockStatus.Pending.ToString()));
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
                    foreach (var tx in block.Transactions)
                    {
                        tx.State = TransactionStatus.Accepted.ToString();
                    }
                }
            }
        }
    }
}