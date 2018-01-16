﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NBitcoin;
using NBitcoin.DataEncoders;
using Stratis.Bitcoin.Base;
using Stratis.Bitcoin.Base.Deployments;
using Stratis.Bitcoin.BlockPulling;
using Stratis.Bitcoin.Configuration;
using Stratis.Bitcoin.Configuration.Logging;
using Stratis.Bitcoin.Connection;
using Stratis.Bitcoin.Features.Consensus;
using Stratis.Bitcoin.Features.Consensus.CoinViews;
using Stratis.Bitcoin.Features.Consensus.Rules;
using Stratis.Bitcoin.Features.MemoryPool;
using Stratis.Bitcoin.Features.MemoryPool.Fee;
using Stratis.Bitcoin.Features.Miner;
using Stratis.Bitcoin.Features.SmartContracts;
using Stratis.Bitcoin.P2P;
using Stratis.Bitcoin.P2P.Peer;
using Stratis.Bitcoin.Utilities;
using Stratis.SmartContracts.ContractValidation;
using Stratis.SmartContracts.State;
using Stratis.SmartContracts.Util;
using Xunit;

namespace Stratis.Bitcoin.IntegrationTests
{
    public class SmartContractTests
    {
        // This is taken from 'MinerTests.cs' and adjusted to use a different block validator.

        private static FeeRate blockMinFeeRate = new FeeRate(PowMining.DefaultBlockMinTxFee);

        public static PowBlockAssembler AssemblerForTest(TestContext testContext)
        {
            AssemblerOptions options = new AssemblerOptions();

            options.BlockMaxWeight = testContext.network.Consensus.Option<PowConsensusOptions>().MaxBlockWeight;
            options.BlockMaxSize = testContext.network.Consensus.Option<PowConsensusOptions>().MaxBlockSerializedSize;
            options.BlockMinFeeRate = blockMinFeeRate;

            return new PowBlockAssembler(testContext.consensus, testContext.network, testContext.mempoolLock, testContext.mempool, testContext.date, testContext.chain.Tip, new LoggerFactory(), options);
        }

        public class Blockinfo
        {
            public int extranonce;
            public uint nonce;
        }

        public static long[,] blockinfoarr =
        {
            {4, 0xa4a3e223}, {2, 0x15c32f9e}, {1, 0x0375b547}, {1, 0x7004a8a5},
            {2, 0xce440296}, {2, 0x52cfe198}, {1, 0x77a72cd0}, {2, 0xbb5d6f84},
            {2, 0x83f30c2c}, {1, 0x48a73d5b}, {1, 0xef7dcd01}, {2, 0x6809c6c4},
            {2, 0x0883ab3c}, {1, 0x087bbbe2}, {2, 0x2104a814}, {2, 0xdffb6daa},
            {1, 0xee8a0a08}, {2, 0xba4237c1}, {1, 0xa70349dc}, {1, 0x344722bb},
            {3, 0xd6294733}, {2, 0xec9f5c94}, {2, 0xca2fbc28}, {1, 0x6ba4f406},
            {2, 0x015d4532}, {1, 0x6e119b7c}, {2, 0x43e8f314}, {2, 0x27962f38},
            {2, 0xb571b51b}, {2, 0xb36bee23}, {2, 0xd17924a8}, {2, 0x6bc212d9},
            {1, 0x630d4948}, {2, 0x9a4c4ebb}, {2, 0x554be537}, {1, 0xd63ddfc7},
            {2, 0xa10acc11}, {1, 0x759a8363}, {2, 0xfb73090d}, {1, 0xe82c6a34},
            {1, 0xe33e92d7}, {3, 0x658ef5cb}, {2, 0xba32ff22}, {5, 0x0227a10c},
            {1, 0xa9a70155}, {5, 0xd096d809}, {1, 0x37176174}, {1, 0x830b8d0f},
            {1, 0xc6e3910e}, {2, 0x823f3ca8}, {1, 0x99850849}, {1, 0x7521fb81},
            {1, 0xaacaabab}, {1, 0xd645a2eb}, {5, 0x7aea1781}, {5, 0x9d6e4b78},
            {1, 0x4ce90fd8}, {1, 0xabdc832d}, {6, 0x4a34f32a}, {2, 0xf2524c1c},
            {2, 0x1bbeb08a}, {1, 0xad47f480}, {1, 0x9f026aeb}, {1, 0x15a95049},
            {2, 0xd1cb95b2}, {2, 0xf84bbda5}, {1, 0x0fa62cd1}, {1, 0xe05f9169},
            {1, 0x78d194a9}, {5, 0x3e38147b}, {5, 0x737ba0d4}, {1, 0x63378e10},
            {1, 0x6d5f91cf}, {2, 0x88612eb8}, {2, 0xe9639484}, {1, 0xb7fabc9d},
            {2, 0x19b01592}, {1, 0x5a90dd31}, {2, 0x5bd7e028}, {2, 0x94d00323},
            {1, 0xa9b9c01a}, {1, 0x3a40de61}, {1, 0x56e7eec7}, {5, 0x859f7ef6},
            {1, 0xfd8e5630}, {1, 0x2b0c9f7f}, {1, 0xba700e26}, {1, 0x7170a408},
            {1, 0x70de86a8}, {1, 0x74d64cd5}, {1, 0x49e738a1}, {2, 0x6910b602},
            {0, 0x643c565f}, {1, 0x54264b3f}, {2, 0x97ea6396}, {2, 0x55174459},
            {2, 0x03e8779a}, {1, 0x98f34d8f}, {1, 0xc07b2b07}, {1, 0xdfe29668},
            {1, 0x3141c7c1}, {1, 0xb3b595f4}, {1, 0x735abf08}, {5, 0x623bfbce},
            {2, 0xd351e722}, {1, 0xf4ca48c9}, {1, 0x5b19c670}, {1, 0xa164bf0e},
            {2, 0xbbbeb305}, {2, 0xfe1c810a}
        };

        public ChainedBlock CreateBlockIndex(ChainedBlock prev)
        {
            ChainedBlock index = new ChainedBlock(new BlockHeader(), new BlockHeader().GetHash(), prev);
            return index;
        }

        public bool TestSequenceLocks(TestContext testContext, ChainedBlock chainedBlock, Transaction tx, Transaction.LockTimeFlags flags, LockPoints uselock = null)
        {
            var context = new MempoolValidationContext(tx, new MempoolValidationState(false));
            context.View = new MempoolCoinView(testContext.cachedCoinView, testContext.mempool, testContext.mempoolLock, null);
            context.View.LoadViewAsync(tx).GetAwaiter().GetResult();
            return MempoolValidator.CheckSequenceLocks(chainedBlock, context, flags, uselock, false);
        }
        public class TestContext
        {
            public List<Blockinfo> blockinfo;
            public Network network;
            public Script scriptPubKey;
            public BlockTemplate newBlock;
            public Transaction tx, tx2;
            public Script script;
            public uint256 hash;
            public TestMemPoolEntryHelper entry;
            public ConcurrentChain chain;
            public ConsensusLoop consensus;
            public DateTimeProvider date;
            public TxMempool mempool;
            public MempoolSchedulerLock mempoolLock;
            public List<Transaction> txFirst;
            public Money BLOCKSUBSIDY = 50 * Money.COIN;
            public Money LOWFEE = Money.CENT;
            public Money HIGHFEE = Money.COIN;
            public Money HIGHERFEE = 4 * Money.COIN;
            public int baseheight;
            public CachedCoinView cachedCoinView;
            public Repository state;

            private bool useCheckpoints = true;

            public TestContext()
            {
            }

            public async Task InitializeAsync()
            {
                this.blockinfo = new List<Blockinfo>();
                var lst = blockinfoarr.Cast<long>().ToList();
                for (int i = 0; i < lst.Count; i += 2)
                    this.blockinfo.Add(new Blockinfo { extranonce = (int)lst[i], nonce = (uint)lst[i + 1] });

                // Note that by default, these tests run with size accounting enabled.
                this.network = Network.Main;
                var hex = Encoders.Hex.DecodeData("04678afdb0fe5548271967f1a67130b7105cd6a828e03909a67962e0ea1f61deb649f6bc3f4cef38c4f35504e51ec112de5c384df7ba0b8d578a4c702b6bf11d5f");
                this.scriptPubKey = new Script(new[] { Op.GetPushOp(hex), OpcodeType.OP_CHECKSIG });
                this.newBlock = new BlockTemplate();

                this.entry = new TestMemPoolEntryHelper();
                this.chain = new ConcurrentChain(this.network);
                this.network.Consensus.Options = new PowConsensusOptions();
                IDateTimeProvider dateTimeProvider = DateTimeProvider.Default;

                this.cachedCoinView = new CachedCoinView(new InMemoryCoinView(this.chain.Tip.HashBlock), dateTimeProvider, new LoggerFactory());

                var loggerFactory = new ExtendedLoggerFactory();
                loggerFactory.AddConsoleWithFilters();

                var nodeSettings = NodeSettings.Default();
                var consensusSettings = new ConsensusSettings(nodeSettings, loggerFactory)
                {
                    UseCheckpoints = this.useCheckpoints
                };

                this.state = new Repository();
                this.state.Refresh(); // just for unit tests
                SmartContractDecompiler smartContractDecompiler = new SmartContractDecompiler();
                SmartContractValidator validator = new SmartContractValidator(new List<ISmartContractValidator>
                        {
                            new SmartContractFormatValidator(),
                            new SmartContractDeterminismValidator()
                        });
                SmartContractGasInjector gasInjector = new SmartContractGasInjector();
                SCConsensusValidator consensusValidator = new SCConsensusValidator(this.network, new Checkpoints(), dateTimeProvider, loggerFactory, this.state, smartContractDecompiler, validator, gasInjector);
                NetworkPeerFactory networkPeerFactory = new NetworkPeerFactory(this.network, dateTimeProvider, loggerFactory);

                var peerAddressManager = new PeerAddressManager(nodeSettings.DataFolder, loggerFactory);
                var peerDiscovery = new PeerDiscovery(new AsyncLoopFactory(loggerFactory), loggerFactory, Network.Main, networkPeerFactory, new NodeLifetime(), nodeSettings, peerAddressManager);
                var connectionManager = new ConnectionManager(dateTimeProvider, loggerFactory, this.network, networkPeerFactory, nodeSettings, new NodeLifetime(), new NetworkPeerConnectionParameters(), peerAddressManager, new IPeerConnector[] { }, peerDiscovery);

                LookaheadBlockPuller blockPuller = new LookaheadBlockPuller(this.chain, connectionManager, new LoggerFactory());
                PeerBanning peerBanning = new PeerBanning(connectionManager, loggerFactory, dateTimeProvider, nodeSettings);
                NodeDeployments deployments = new NodeDeployments(this.network, this.chain);
                ConsensusRules consensusRules = new ConsensusRules(this.network, loggerFactory, dateTimeProvider, this.chain, deployments, consensusSettings, new Checkpoints()).Register(new FullNodeBuilderConsensusExtension.CoreConsensusRules());
                this.consensus = new ConsensusLoop(new AsyncLoopFactory(loggerFactory), consensusValidator, new NodeLifetime(), this.chain, this.cachedCoinView, blockPuller, new NodeDeployments(this.network, this.chain), loggerFactory, new ChainState(new InvalidBlockHashStore(dateTimeProvider)), connectionManager, dateTimeProvider, new Signals.Signals(), consensusSettings, nodeSettings, peerBanning, consensusRules);
                await this.consensus.StartAsync();

                this.entry.Fee(11);
                this.entry.Height(11);
                var date1 = new MemoryPoolTests.DateTimeProviderSet();
                date1.time = dateTimeProvider.GetTime();
                date1.timeutc = dateTimeProvider.GetUtcNow();
                this.date = date1;
                this.mempool = new TxMempool(dateTimeProvider, new BlockPolicyEstimator(new MempoolSettings(nodeSettings), new LoggerFactory(), nodeSettings), new LoggerFactory(), nodeSettings); ;
                this.mempoolLock = new MempoolSchedulerLock();

                // Simple block creation, nothing special yet:
                this.newBlock = AssemblerForTest(this).CreateNewBlock(this.scriptPubKey);
                this.chain.SetTip(this.newBlock.Block.Header);
                await this.consensus.ValidateAndExecuteBlockAsync(new RuleContext(new BlockValidationContext { Block = this.newBlock.Block }, this.network.Consensus, this.consensus.Tip) { CheckPow = false, CheckMerkleRoot = false });

                // We can't make transactions until we have inputs
                // Therefore, load 100 blocks :)
                this.baseheight = 0;
                List<NBitcoin.Block> blocks = new List<NBitcoin.Block>();
                this.txFirst = new List<Transaction>();
                for (int i = 0; i < this.blockinfo.Count; ++i)
                {
                    var pblock = this.newBlock.Block.Clone(); // pointer for convenience
                    pblock.Header.HashPrevBlock = this.chain.Tip.HashBlock;
                    pblock.Header.Version = 1;
                    pblock.Header.Time = Utils.DateTimeToUnixTime(this.chain.Tip.GetMedianTimePast()) + 1;
                    Transaction txCoinbase = pblock.Transactions[0].Clone();
                    txCoinbase.Inputs.Clear();
                    txCoinbase.Version = 1;
                    txCoinbase.AddInput(new TxIn(new Script(new[] { Op.GetPushOp(this.blockinfo[i].extranonce), Op.GetPushOp(this.chain.Height) })));
                    // Ignore the (optional) segwit commitment added by CreateNewBlock (as the hardcoded nonces don't account for this)
                    txCoinbase.AddOutput(new TxOut(Money.Zero, new Script()));
                    pblock.Transactions[0] = txCoinbase;

                    if (this.txFirst.Count == 0)
                        this.baseheight = this.chain.Height;
                    if (this.txFirst.Count < 4)
                        this.txFirst.Add(pblock.Transactions[0]);
                    pblock.UpdateMerkleRoot();

                    pblock.Header.Nonce = this.blockinfo[i].nonce;

                    this.chain.SetTip(pblock.Header);
                    await this.consensus.ValidateAndExecuteBlockAsync(new RuleContext(new BlockValidationContext { Block = pblock }, this.network.Consensus, this.consensus.Tip) { CheckPow = false, CheckMerkleRoot = false });
                    blocks.Add(pblock);
                }

                // Just to make sure we can still make simple blocks
                this.newBlock = AssemblerForTest(this).CreateNewBlock(this.scriptPubKey);
                Assert.NotNull(this.newBlock);
            }

            internal TestContext WithoutCheckpoints()
            {
                this.useCheckpoints = false;
                return this;
            }
        }

        [Fact]
        public async Task TestSmartContractConsensusValidatorAsync()
        {
            TestContext context = new TestContext();
            await context.InitializeAsync();

            TestMemPoolEntryHelper entry = new TestMemPoolEntryHelper();

            Transaction tx = new Transaction();
            tx.AddInput(new TxIn(new OutPoint(context.txFirst[0].GetHash(), 0), new Script(OpcodeType.OP_1)));

            var contractTransaction = new SCTransaction
            {
                VmVersion = 1,
                GasLimit = 500000,
                GasPrice = 1,
                ContractCode = GetFileDllHelper.GetAssemblyBytesFromFile("SmartContracts/Token.cs"),
                OpCodeType = OpcodeType.OP_CREATECONTRACT
            };
            tx.AddOutput(new TxOut(new Money(5000000000L - 10000), new Script(contractTransaction.ToBytes())));

            var outTest = tx.Outputs.FirstOrDefault();
            bool result = outTest.ScriptPubKey.IsSmartContractExec;
            // This tx has a low fee: 1000 satoshis
            uint256 hashParentTx = tx.GetHash();
            context.mempool.AddUnchecked(hashParentTx, entry.Fee(10000).Time(context.date.GetTime()).SpendsCoinbase(true).FromTx(tx));
            var pblocktemplate = AssemblerForTest(context).CreateNewBlock(context.scriptPubKey);
            context.chain.SetTip(pblocktemplate.Block.Header);
            await context.consensus.ValidateAndExecuteBlockAsync(new RuleContext(new BlockValidationContext { Block = pblocktemplate.Block }, context.network.Consensus, context.consensus.Tip) { CheckPow = false, CheckMerkleRoot = false });
            var hopefullyOwner = context.state.GetObject<uint160>(0, "Owner");

        }
    }
}
