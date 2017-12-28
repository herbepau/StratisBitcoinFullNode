﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;

namespace Stratis.Bitcoin.Features.Consensus
{
    public interface IConsensusLoop
    {
        Task StartAsync();
        void Stop();
        Task AcceptBlockAsync(BlockValidationContext blockValidationContext);
        void ValidateBlock(RuleContext context, bool skipRules = false);
        Task ValidateAndExecuteBlockAsync(RuleContext context);
        Task FlushAsync(bool force);
        uint256[] GetIdsToFetch(Block block, bool enforceBIP30);
    }
}
