using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.Options;
using Starter.Common.DomainTaskStatus;
using Starter.DAL.Entities;
using Starter.DAL.Infrastructure;
using Starter.Services.BankAccount.Models;
using Starter.Services.Providers;
using Starter.Services.Transactions.Models;
using Starter.Services.TwoFactorAuth.TOTP;

namespace Starter.Services.BankAccount
{
    public class BankAccountService : IBankAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITotpProvider _totpProvider;
        private readonly IAuthenticatedUser _user;
        private readonly IMapper _mapper;
        private readonly DomainTaskStatus _taskStatus;
        private const int MinAccountIdLength = 10;

        public BankAccountService(IUnitOfWork unitOfWork,
            ITotpProvider totpProvider,
            IAuthenticatedUser user,
            IMapper mapper,
            DomainTaskStatus taskStatus)
        {
            _unitOfWork = unitOfWork;
            _totpProvider = totpProvider;
            _user = user;
            _mapper = mapper;
            _taskStatus = taskStatus;
        }

        public void CloseAccount(string id, string code)
        {
            var account = _unitOfWork.Repository<BankAccountEntity>().Include(x => x.Owner, x => x.Owner.TwoFactorAuth)
                .FirstOrDefault(x => x.Owner.Id == _user.Id && x.Id == id);

            if (account == null)
            {
                _taskStatus.AddUnkeyedError("invalid account");
                return;
            }

            if (_totpProvider.Verify(account.Owner.TwoFactorAuth.Secret, code))
            {
                _taskStatus.AddUnkeyedError("invalid code");
                return;
            }

            account.Status = BankAccountStatus.Closed.ToString();
            _unitOfWork.SaveChanges();
        }

        public BankAccountModel CreateBankAccount(CreateBankAccountModel account)
        {
            if (!_user.IsAuthenticated)
            {
                _taskStatus.AddUnkeyedError("user is not authorized");
                return null;
            }

            var user = _unitOfWork.Repository<UserEntity>()
                .Include(x => x.TwoFactorAuth)
                .FirstOrDefault(x => x.Id == _user.Id);

            if (!_totpProvider.Verify(user.TwoFactorAuth?.Secret, account.Code))
            {
                _taskStatus.AddUnkeyedError("invalid code");
                return null;
            }

            var entityToInsert = new BankAccountEntity
            {
                Id = GenerateBankId(),
                Owner = user,
                Balance = 0.0m,
                //todo: move to config
                ExpiresAt = DateTimeOffset.Now.AddYears(3),
                OpenedAt = DateTimeOffset.Now,
                Type = account.Type.ToString()
            };

            _unitOfWork.Repository<BankAccountEntity>().Insert(entityToInsert);
            _unitOfWork.SaveChanges();

            //todo: create automapper
            return new BankAccountModel
            {
                Balance = entityToInsert.Balance,
                Id = entityToInsert.Id,
                Type = account.Type
            };
        }

        public IEnumerable<BankAccountModel> GetAccounts(BankAccountType? accountType, BankAccountStatus? status)
        {
            var user = _unitOfWork.Repository<UserEntity>()
                .Include(x => x.Accounts)
                .FirstOrDefault(x => x.Id == _user.Id);

            if (user == null)
            {
                _taskStatus.AddUnkeyedError("invalid user");
                return null;
            }

            return user.Accounts
                .Where(x => accountType == null || x.Type == accountType.ToString())
                .Where(x => status == null || x.Status == status.ToString())
                .Select(x => new BankAccountModel
                {
                    Balance = x.Balance,
                    Id = x.Id,
                    Type = Enum.Parse<BankAccountType>(x.Type)
                });
        }

        //todo: TBD
        public BankAccountModel UpdateAccount(string code)
        {
            throw new NotImplementedException();
        }

        private string GenerateBankId()
        {
            var repository = _unitOfWork.Repository<BankAccountEntity>().Set;

            var nextId = repository.Any() ? int.Parse(repository.Max(x => x.Id)) + 1 : 1;

            if (nextId.ToString().Length < MinAccountIdLength)
            {
                var builder = new StringBuilder(new string('0', MinAccountIdLength - nextId.ToString().Length));
                builder.Append(nextId.ToString());
                return builder.ToString();
            }
            else
            {
                return nextId.ToString();
            }
        }

        public BankAccountDetailedModel GetAccount(string id)
        {
            var account = _unitOfWork.Repository<BankAccountEntity>()
                .Include(x => x.Owner, x => x.ReceivedTransactions, x => x.SentTransactions)
                .FirstOrDefault(x => x.Owner.Id == _user.Id && x.Id == id);

            if (account == null)
            {
                _taskStatus.AddUnkeyedError("invalid account code");
                return null;
            }

            return new BankAccountDetailedModel
            {
                Balance = account.Balance,
                Transactions = account
                    .ReceivedTransactions
                    .Union(account.SentTransactions)
                    .Select(x => _mapper.Map<TransactionDetailedModel>(x))
                    .OrderBy(x => x.SentTime),
                Id = account.Id,
                Type = Enum.Parse<BankAccountType>(account.Type),
                ClosedAt = account.ExpiresAt,
                OpenedAt = account.OpenedAt
            };
        }
    }
}