using System;
using System.Collections.Generic;
using System.Text;
using Starter.Services.TestDataGenerationService.Models;

namespace Starter.Services.TestDataGenerationService
{
    public interface ITestDataGenerationService
    {
        TestUserModel CreateTestUser();
        void GenerateTransactions(string fromAccount = null, string toAccount = null, int number = 200);
    }
}