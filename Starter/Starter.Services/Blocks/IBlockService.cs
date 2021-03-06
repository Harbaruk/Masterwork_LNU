﻿using System;
using System.Collections.Generic;
using System.Text;
using Starter.Services.Blocks.Models;

namespace Starter.Services.Blocks
{
    public interface IBlockService
    {
        BlockModel CreateBlock(CreateBlockModel model);
        IEnumerable<BlockModel> GetBlocks(int take, int skip);
        void VerifyBlock(string blockId, string serverHash);
        UnverifiedBlockModel GetUnverifiedBlock();
        BlockModel GetLastBlock();
        void SaveVerifiedBlock(UnverifiedBlockModel model);
    }
}