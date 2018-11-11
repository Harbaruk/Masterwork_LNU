using System;
using System.Collections.Generic;
using System.Text;
using Starter.Services.BankAccount.Models;

namespace Starter.Services.BankAccount
{
    public interface IBankAccountService
    {
        BankAccountModel CreateBankAccount(CreateBankAccountModel account);
        IEnumerable<BankAccountModel> GetAccounts(BankAccountType? accountType, BankAccountStatus? bankAccountStatus);
        BankAccountDetailedModel GetAccount(string id);
        void CloseAccount(string id, string code);
        BankAccountModel UpdateAccount(string code);
    }
}