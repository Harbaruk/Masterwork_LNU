using System;
using System.Collections.Generic;
using System.Text;

namespace Starter.Services.Blocks
{
    public interface IBlockService
    {
        BlockModel CreateBlock(CreateBlockModel model);
        IEnumerable<BlockModel> GetBlocks(int take, int skip);
        void VerifyBlock(string blockId);
    }
}