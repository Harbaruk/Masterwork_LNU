using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Starter.Services.Blocks.Models;
using Starter.Services.Transactions.Models;

namespace Starter.Services.Mining
{
    public interface IMiningService
    {
        void Run();
    }
}