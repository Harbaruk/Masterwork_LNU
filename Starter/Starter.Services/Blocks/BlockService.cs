using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Options;
using Starter.Common.DomainTaskStatus;
using Starter.DAL.Entities;
using Starter.DAL.Infrastructure;
using Starter.Services.Blocks.Models;
using Starter.Services.Providers;

namespace Starter.Services.Blocks
{
    public class BlockService : IBlockService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenticatedUser _authenticatedUser;
        private readonly IOptions<BlockSettingsOptions> _options;
        private readonly DomainTaskStatus _taskStatus;

        public BlockService(
            IUnitOfWork unitOfWork,
            IAuthenticatedUser authenticatedUser, IOptions<BlockSettingsOptions> options, DomainTaskStatus taskStatus)
        {
            _unitOfWork = unitOfWork;
            _authenticatedUser = authenticatedUser;
            _options = options;
            _taskStatus = taskStatus;
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
            throw new NotImplementedException();
        }

        public void VerifyBlock(string blockId)
        {
            throw new NotImplementedException();
        }
    }
}