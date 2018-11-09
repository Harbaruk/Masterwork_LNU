using System;
using System.Collections.Generic;
using System.Text;

namespace Starter.Services.BankAccount.Models
{
    public class CreateBankAccountModel
    {
        public BankAccountType Type { get; set; }
        public string Code { get; set; }
    }
}