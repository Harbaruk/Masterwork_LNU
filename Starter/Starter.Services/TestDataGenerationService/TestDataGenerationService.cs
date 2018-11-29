using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Starter.DAL.Entities;
using Starter.DAL.Infrastructure;
using Starter.Services.Crypto;
using Starter.Services.Enums;
using Starter.Services.TestDataGenerationService.Models;
using Starter.Services.Transactions.Models;

namespace Starter.Services.TestDataGenerationService
{
    public class TestDataGenerationService : ITestDataGenerationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICryptoContext _cryptoContext;

        public TestDataGenerationService(IUnitOfWork unitOfWork, ICryptoContext cryptoContext)
        {
            _unitOfWork = unitOfWork;
            _cryptoContext = cryptoContext;
        }

        public TestUserModel CreateTestUser()
        {
            var testUsersCount = _unitOfWork.Repository<UserEntity>().Set.Count(x => x.Role == UserRoles.Test.ToString());

            var salt = _cryptoContext.GenerateSaltAsBase64();
            var newUser = new UserEntity
            {
                Email = $"test_user{testUsersCount + 1}@test.com",
                Firstname = $"Test",
                Lastname = $"User",
                Salt = salt,
                Password = Convert.ToBase64String(_cryptoContext.DeriveKey("testuser", salt)),
                IsVerified = true,
                Role = UserRoles.Test.ToString()
            };

            _unitOfWork.Repository<UserEntity>().Insert(newUser);
            return new TestUserModel
            {
                Firstname = newUser.Firstname,
                Id = newUser.Id,
                Lastname = newUser.Lastname,
                Password = "testuser",
                Email = newUser.Email
            };
        }

        public void GenerateTransactions(string fromAccount = null, string toAccount = null, int number = 200)
        {
            var FromAccount = _unitOfWork.Repository<BankAccountEntity>().Include(x => x.Owner)
                .FirstOrDefault(x => x.Owner.Role == UserRoles.Test.ToString() && (fromAccount == null ? true : x.Id == fromAccount));

            var ToAccount = _unitOfWork.Repository<BankAccountEntity>().Include(x => x.Owner)
                .FirstOrDefault(x => x.Owner.Role == UserRoles.Test.ToString() && (toAccount == null ? true : x.Id == toAccount));

            var testData = Enumerable.Repeat(new TransactionEntity
            {
                Date = DateTimeOffset.Now,
                FromAccount = FromAccount,
                ToAccount = ToAccount,
                Money = 500,
                State = TransactionStatus.Pending.ToString(),
                Initiator = FromAccount.Owner,
                Description = "Test"
            }, number);

            _unitOfWork.Repository<TransactionEntity>().InsertRange(testData);
            _unitOfWork.SaveChanges();
        }
    }
}