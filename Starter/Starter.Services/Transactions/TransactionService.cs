using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Starter.Common.DomainTaskStatus;
using Starter.DAL.Entities;
using Starter.DAL.Infrastructure;
using Starter.Services.BankAccount.Models;
using Starter.Services.Providers;
using Starter.Services.Transactions.Models;

namespace Starter.Services.Transactions
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DomainTaskStatus _taskStatus;
        private readonly IAuthenticatedUser _authenticatedUser;
        private readonly IMapper _mapper;

        public TransactionService(IUnitOfWork unitOfWork, DomainTaskStatus taskStatus, IAuthenticatedUser authenticatedUser, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _taskStatus = taskStatus;
            _authenticatedUser = authenticatedUser;
            _mapper = mapper;
        }

        public TransactionModel CreateTransaction(CreateTransactionModel model)
        {
            var account = _unitOfWork.Repository<BankAccountEntity>()
                .Include(x => x.Owner, x => x.SentTransactions)
                .FirstOrDefault(x => x.Owner.Id == _authenticatedUser.Id && x.Id == model.FromAccount);

            var toAccount = _unitOfWork.Repository<BankAccountEntity>()
                .Set
                .FirstOrDefault(x => x.Id == model.ToAccount);

            if (account == null)
            {
                _taskStatus.AddUnkeyedError("invalid account id");
                return null;
            }

            if (Enum.Parse<BankAccountType>(account.Type) == BankAccountType.Deposit && model.Money > account.Balance)
            {
                _taskStatus.AddUnkeyedError("not enough money");
                return null;
            }

            var transaction = new TransactionEntity
            {
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
                .Include(x => x.Initiator, x => x.FromAccount, x => x.ToAccount)
                .FirstOrDefault(x => x.Initiator.Id == _authenticatedUser.Id && x.Id == id);

            if (transaction == null)
            {
                _taskStatus.AddUnkeyedError("transaction id is invalid");
                return null;
            }

            return _mapper.Map<TransactionDetailedModel>(transaction);
        }

        public IEnumerable<TransactionModel> GetTransactions(TransactionStatus? status)
        {
            throw new NotImplementedException();
        }
    }
}