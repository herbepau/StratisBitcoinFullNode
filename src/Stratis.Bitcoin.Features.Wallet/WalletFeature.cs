using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using NBitcoin;
using Stratis.Bitcoin.Builder;
using Stratis.Bitcoin.Builder.Feature;
using Stratis.Bitcoin.Configuration;
using Stratis.Bitcoin.Configuration.Logging;
using Stratis.Bitcoin.Connection;
using Stratis.Bitcoin.Features.BlockStore;
using Stratis.Bitcoin.Features.MemoryPool;
using Stratis.Bitcoin.Features.RPC;
using Stratis.Bitcoin.Features.Wallet.Broadcasting;
using Stratis.Bitcoin.Features.Wallet.Controllers;
using Stratis.Bitcoin.Features.Wallet.Interfaces;
using Stratis.Bitcoin.Features.Wallet.Notifications;
using Stratis.Bitcoin.Interfaces;
using Stratis.Bitcoin.Statistics;
using Stratis.Bitcoin.Statistics.Interfaces;
using Stratis.Bitcoin.Utilities.Extensions;

namespace Stratis.Bitcoin.Features.Wallet
{
    /// <summary>
    /// Wallet feature for the full node.
    /// </summary>
    /// <seealso cref="Stratis.Bitcoin.Builder.Feature.FullNodeFeature" />
    /// <seealso cref="Stratis.Bitcoin.Interfaces.INodeStats" />
    public class WalletFeature : FullNodeFeature, INodeStats, IFeatureStats
    {
        private readonly IWalletSyncManager walletSyncManager;

        private readonly IWalletManager walletManager;

        private readonly Signals.Signals signals;

        private IDisposable blockSubscriberDisposable;

        private IDisposable transactionSubscriberDisposable;

        private ConcurrentChain chain;

        private readonly IConnectionManager connectionManager;

        private readonly BroadcasterBehavior broadcasterBehavior;

        private readonly NodeSettings nodeSettings;

        private readonly WalletSettings walletSettings;

        private readonly IStatisticsService statisticsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="WalletFeature"/> class.
        /// </summary>
        /// <param name="walletSyncManager">The synchronization manager for the wallet, tasked with keeping the wallet synced with the network.</param>
        /// <param name="walletManager">The wallet manager.</param>
        /// <param name="signals">The signals responsible for receiving blocks and transactions from the network.</param>
        /// <param name="chain">The chain of blocks.</param>
        /// <param name="connectionManager">The connection manager.</param>
        /// <param name="broadcasterBehavior">The broadcaster behavior.</param>
        /// <param name="nodeSettings">The settings for the node.</param>
        /// <param name="walletSettings">The settings for the wallet.</param>
        public WalletFeature(
            IWalletSyncManager walletSyncManager,
            IWalletManager walletManager,
            Signals.Signals signals,
            ConcurrentChain chain,
            IConnectionManager connectionManager,
            BroadcasterBehavior broadcasterBehavior,
            NodeSettings nodeSettings,
            WalletSettings walletSettings)
        {
            this.walletSyncManager = walletSyncManager;
            this.walletManager = walletManager;
            this.signals = signals;
            this.chain = chain;
            this.connectionManager = connectionManager;
            this.broadcasterBehavior = broadcasterBehavior;
            this.nodeSettings = nodeSettings;
            this.walletSettings = walletSettings;
        }

        /// <summary>
        /// Prints command-line help.
        /// </summary>
        /// <param name="network">The network to extract values from.</param>
        public static void PrintHelp(Network network)
        {
            WalletSettings.PrintHelp(network);
        }

        /// <summary>
        /// Get the default configuration.
        /// </summary>
        /// <param name="builder">The string builder to add the settings to.</param>
        /// <param name="network">The network to base the defaults off.</param>
        public static void BuildDefaultConfigurationFile(StringBuilder builder, Network network)
        {
            WalletSettings.BuildDefaultConfigurationFile(builder, network);
        }

        /// <inheritdoc />
        public void AddNodeStats(StringBuilder benchLogs)
        {
            var statistics = this.NodeStatistics;
            if (statistics.IsEmpty())
                return;

            IStatistic height = statistics.First(), hashBlock = statistics.Last();

            benchLogs.AppendLine($"{height.Id}: ".PadRight(LoggingConfiguration.ColumnLength + 1) + height.Value.PadRight(8) +
                                (!string.IsNullOrEmpty(hashBlock.Value) ? $"{hashBlock.Id}".PadRight(LoggingConfiguration.ColumnLength - 1) + hashBlock.Value : string.Empty));
        }

        public IEnumerable<IStatistic> NodeStatistics
        {
            get
            {                
                var walletManager = this.walletManager as WalletManager;
                if (walletManager != null)
                {
                    int height = walletManager.LastBlockHeight();
                    ChainedHeader block = this.chain.GetBlock(height);
                    uint256 hashBlock = block == null ? 0 : block.HashBlock;

                    yield return new Statistic("Wallet.Height", walletManager.ContainsWallets ? height.ToString() : "No Wallet", "Wallet Height");
                    yield return new Statistic("Wallet.Hash", walletManager.ContainsWallets ? hashBlock.ToString() : string.Empty, "Wallet Hash");
                }
            }
        }

        /// <inheritdoc />
        public void AddFeatureStats(StringBuilder benchLog)
        {
            IStatisticGroup statisticsGroup = this.FeatureStatistics;
            if (statisticsGroup == null)
                return;

            benchLog.AppendLine();
            benchLog.AppendLine("======Wallets======");

            foreach (IStatisticGroup walletGroup in statisticsGroup.Groups)
                benchLog.AppendLine("Wallet: " + (walletGroup.GroupName + ",").PadRight(LoggingConfiguration.ColumnLength) + " Confirmed balance: " + walletGroup.Statistics.First().Value);
        }

        public IStatisticGroup FeatureStatistics
        {
            get
            {
                IEnumerable<string> walletNames = this.walletManager.GetWalletsNames().ToList();
                if (walletNames.Any())
                {
                    var statisticGroup = new StatisticGroup("Wallets");
                    foreach (string walletName in walletNames)
                    {
                        IEnumerable<UnspentOutputReference> items = this.walletManager.GetSpendableTransactionsInWallet(walletName, 1);
                        var confirmedBalance = new Money(items.Sum(s => s.Transaction.Amount)).ToString();

                        statisticGroup.AddGroup(walletName)
                                      .Apply(new Statistic("ConfirmedBalance", confirmedBalance, "Confirmed Balance"));
                    }
                    return statisticGroup;
                }

                return null;
            }
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            // subscribe to receiving blocks and transactions
            this.blockSubscriberDisposable = this.signals.SubscribeForBlocks(new BlockObserver(this.walletSyncManager));
            this.transactionSubscriberDisposable = this.signals.SubscribeForTransactions(new TransactionObserver(this.walletSyncManager));

            this.walletManager.Start();
            this.walletSyncManager.Start();

            this.connectionManager.Parameters.TemplateBehaviors.Add(this.broadcasterBehavior);
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            this.blockSubscriberDisposable.Dispose();
            this.transactionSubscriberDisposable.Dispose();

            this.walletManager.Stop();
            this.walletSyncManager.Stop();
        }
    }

    /// <summary>
    /// A class providing extension methods for <see cref="IFullNodeBuilder"/>.
    /// </summary>
    public static class FullNodeBuilderWalletExtension
    {
        public static IFullNodeBuilder UseWallet(this IFullNodeBuilder fullNodeBuilder)
        {
            LoggingConfiguration.RegisterFeatureNamespace<WalletFeature>("wallet");

            fullNodeBuilder.ConfigureFeature(features =>
            {
                features
                .AddFeature<WalletFeature>()
                .DependOn<MempoolFeature>()
                .DependOn<BlockStoreFeature>()
                .DependOn<RPCFeature>()
                .FeatureServices(services =>
                    {
                        services.AddSingleton<IWalletSyncManager, WalletSyncManager>();
                        services.AddSingleton<IWalletTransactionHandler, WalletTransactionHandler>();
                        services.AddSingleton<IWalletManager, WalletManager>();
                        services.AddSingleton<IWalletFeePolicy, WalletFeePolicy>();
                        services.AddSingleton<WalletController>();
                        services.AddSingleton<WalletRPCController>();
                        services.AddSingleton<IBroadcasterManager, FullNodeBroadcasterManager>();
                        services.AddSingleton<BroadcasterBehavior>();
                        services.AddSingleton<WalletSettings>();
                    });
            });

            return fullNodeBuilder;
        }
    }
}
