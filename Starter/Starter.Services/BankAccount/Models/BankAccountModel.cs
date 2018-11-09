using System;
using System.Collections.Generic;
using System.Text;

namespace Starter.Services.BankAccount.Models
{
    public class BankAccountModel
    {
        public string Id { get; set; }
        public BankAccountType Type { get; set; }
        public decimal Balance { get; set; }
    }
}