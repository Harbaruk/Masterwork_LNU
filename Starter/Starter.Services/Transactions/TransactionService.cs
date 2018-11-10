using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public TransactionService(IUnitOfWork unitOfWork, DomainTaskStatus taskStatus, IAuthenticatedUser authenticatedUser)
        {
            _unitOfWork = unitOfWork;
            _taskStatus = taskStatus;
            _authenticatedUser = authenticatedUser;
        }

        public TransactionModel CreateTransaction(CreateTransactionModel model)
        {
            var account = _unitOfWork.Repository<BankAccountEntity>()
                .Include(x => x.Owner, x => x.SentTransactions)
                .FirstOrDefault(x => x.Owner.Id == _authenticatedUser.Id && x.Id == model.FromAccount);

            var toAccount = _unitOfWork.Repository<BankAccountEntity>()
                .Set
                .FirstOrDefault(x => x.Id == model.ToAccount);

            if (account == null || toAccount == null)
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

            return new TransactionModel
            {
                Balance = transaction.Money,
                Date = transaction.Date,
                Id = transaction.Id,
                Status = Enum.Parse<TransactionStatus>(transaction.State)
            };
        }

        public TransactionDetailedModel GetTransaction(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TransactionModel> GetTransactions(TransactionStatus? status)
        {
            throw new NotImplementedException();
        }
    }
}