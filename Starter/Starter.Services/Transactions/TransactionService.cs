﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.Options;
using Starter.Common.DomainTaskStatus;
using Starter.DAL.Entities;
using Starter.DAL.Infrastructure;
using Starter.Services.BankAccount.Models;
using Starter.Services.Blocks;
using Starter.Services.Providers;
using Starter.Services.Transactions.Models;

namespace Starter.Services.Transactions
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptions<BlockSettingsOptions> _blockOptions;
        private readonly DomainTaskStatus _taskStatus;
        private readonly IAuthenticatedUser _authenticatedUser;
        private readonly IMapper _mapper;

        public TransactionService(IUnitOfWork unitOfWork, IOptions<BlockSettingsOptions> blockOptions, DomainTaskStatus taskStatus, IAuthenticatedUser authenticatedUser, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _blockOptions = blockOptions;
            _taskStatus = taskStatus;
            _authenticatedUser = authenticatedUser;
            _mapper = mapper;
        }

        public int CountUnverifiedTransactions()
        {
            return _unitOfWork.Repository<TransactionEntity>()
                .Set.Count(x => x.State == TransactionStatus.Pending.ToString());
        }

        public TransactionModel CreateTransaction(CreateTransactionModel model)
        {
            var account = _unitOfWork.Repository<BankAccountEntity>()
                .Include(x => x.Owner, x => x.SentTransactions)
                .FirstOrDefault(x => x.Owner.Id == _authenticatedUser.Id && x.Id == model.FromAccount);

            var toAccount = _unitOfWork.Repository<BankAccountEntity>()
                .Include(x => x.Owner)
                .FirstOrDefault(x => x.Id == model.ToAccount);

            if (model.Money < 0)
            {
                _taskStatus.AddUnkeyedError("invalid amount of money");
                return null;
            }

            if (Enum.Parse<BankAccountType>(account.Type) == BankAccountType.Deposit && model.Money > account.Balance)
            {
                _taskStatus.AddUnkeyedError("not enough money");
                return null;
            }

            var transaction = new TransactionEntity
            {
                Id = Guid.NewGuid().ToString(),
                Date = DateTimeOffset.Now,
                FromAccount = account,
                ToAccount = toAccount,
                Initiator = account.Owner,
                Description = model.Desctiption,
                Money = model.Money,
                State = TransactionStatus.Pending.ToString()
            };

            _unitOfWork.Repository<TransactionEntity>().Insert(transaction);
            _unitOfWork.SaveChanges();

            return _mapper.Map<TransactionModel>(transaction);
        }

        public TransactionDetailedModel GetTransaction(string id)
        {
            var transaction = _unitOfWork.Repository<TransactionEntity>()
                .Include(x => x.Initiator, x => x.FromAccount, x => x.ToAccount, x => x.Block)
                .FirstOrDefault(x => x.Initiator.Id == _authenticatedUser.Id && x.Id == id);

            if (transaction == null)
            {
                _taskStatus.AddUnkeyedError("transaction id is invalid");
                return null;
            }

            return _mapper.Map<TransactionDetailedModel>(transaction);
        }

        public IEnumerable<TransactionModel> GetTransactions(string bankAccountId, TransactionStatus? status)
        {
            return _unitOfWork.Repository<TransactionEntity>()
                .Include(x => x.FromAccount.Owner,
                         x => x.ToAccount.Owner)
                .Where(x => (x.FromAccount.Id == bankAccountId && x.FromAccount.Owner.Id == _authenticatedUser.Id)
                         || (x.ToAccount.Id == bankAccountId && x.ToAccount.Owner.Id == _authenticatedUser.Id))
                .Where(x => status == null || x.State == status.ToString())
                .Select(x => _mapper.Map<TransactionModel>(x));
        }

        public IEnumerable<TransactionDetailedModel> GetUnverifiedTransactions(int number = 0)
        {
            return _unitOfWork.Repository<TransactionEntity>()
                .Include(x => x.Block, x => x.FromAccount, x => x.ToAccount)
                .Where(x => x.State == TransactionStatus.Pending.ToString())
                .Take(number == 0 ? _blockOptions.Value.BlockSize : number)
                .Select(x => _mapper.Map<TransactionDetailedModel>(x));
        }
    }
}