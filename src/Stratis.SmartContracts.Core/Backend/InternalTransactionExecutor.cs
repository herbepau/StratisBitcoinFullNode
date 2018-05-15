﻿using Microsoft.Extensions.Logging;
using NBitcoin;
using Stratis.SmartContracts.Core.Exceptions;
using Stratis.SmartContracts.Core.State;
using Stratis.SmartContracts.Core.State.AccountAbstractionLayer;

namespace Stratis.SmartContracts.Core.Backend
{
    ///<inheritdoc/>
    public sealed class InternalTransactionExecutor : IInternalTransactionExecutor
    {
        private readonly IContractStateRepository contractStateRepository;
        private readonly InternalTransferList internalTransferList;
        private readonly IKeyEncodingStrategy keyEncodingStrategy;
        private readonly ILogger logger;
        private readonly ILoggerFactory loggerFactory;
        private readonly Network network;

        public InternalTransactionExecutor(
            IContractStateRepository contractStateRepository,
            InternalTransferList internalTransferList,
            IKeyEncodingStrategy keyEncodingStrategy,
            ILoggerFactory loggerFactory,
            Network network)
        {
            this.contractStateRepository = contractStateRepository;
            this.internalTransferList = internalTransferList;
            this.keyEncodingStrategy = keyEncodingStrategy;
            this.loggerFactory = loggerFactory;
            this.logger = loggerFactory.CreateLogger(this.GetType());
            this.network = network;
        }

        ///<inheritdoc/>
        public ITransferResult TransferFunds(ISmartContractState smartContractState, Address addressTo, ulong amountToTransfer, TransferFundsToContract contractDetails)
        {
            this.logger.LogTrace("({0}:{1},{2}:{3})", nameof(addressTo), addressTo, nameof(amountToTransfer), amountToTransfer);

            // TODO: The act of calling this should cost a lot of gas!
            var balance = smartContractState.GetBalance();
            if (balance < amountToTransfer)
            {
                this.logger.LogTrace("(-)[INSUFFICIENT_BALANCE] {0}={1}", nameof(balance), balance);
                throw new InsufficientBalanceException();
            }

            // Discern whether this is a contract or an ordinary address.
            byte[] contractCode = this.contractStateRepository.GetCode(addressTo.ToUint160(this.network));
            if (contractCode == null || contractCode.Length == 0)
            {
                this.internalTransferList.Add(new TransferInfo
                {
                    From = smartContractState.Message.ContractAddress.ToUint160(this.network),
                    To = addressTo.ToUint160(this.network),
                    Value = amountToTransfer
                });

                this.logger.LogTrace("Transfer {0} from {1} to {2}.", smartContractState.Message.ContractAddress.ToUint160(this.network), addressTo.ToUint160(this.network), amountToTransfer);
                this.logger.LogTrace("(-)[TRANSFER_TO_SENDER]");
                return TransferResult.Empty();
            }

            this.logger.LogTrace("(-)[TRANSFER_TO_CONTRACT]");

            return ExecuteTransferFundsToContract(contractCode, smartContractState, addressTo, amountToTransfer, contractDetails);
        }

        /// <summary>
        /// If the address to where the funds will be tranferred to is a contract, instantiate and execute it.
        /// </summary>
        private ITransferResult ExecuteTransferFundsToContract(byte[] contractCode, ISmartContractState smartContractState, Address addressTo, ulong amountToTransfer, TransferFundsToContract contractDetails)
        {
            this.logger.LogTrace("({0}:{1},{2}:{3})", nameof(addressTo), addressTo, nameof(amountToTransfer), amountToTransfer);

            IContractStateRepository track = this.contractStateRepository.StartTracking();
            IPersistenceStrategy persistenceStrategy = new MeteredPersistenceStrategy(track, smartContractState.GasMeter, this.keyEncodingStrategy);
            IPersistentState newPersistentState = new PersistentState(persistenceStrategy, addressTo.ToUint160(this.network), this.network);

            var newMessage = new Message(addressTo, smartContractState.Message.ContractAddress, amountToTransfer, (Gas)(smartContractState.Message.GasLimit - smartContractState.GasMeter.GasConsumed));

            ISmartContractExecutionContext newContext = new SmartContractExecutionContext(smartContractState.Block, newMessage, addressTo.ToUint160(this.network), 0, contractDetails.MethodParameters);

            ISmartContractVirtualMachine vm = new ReflectionVirtualMachine(new InternalTransactionExecutorFactory(this.keyEncodingStrategy, this.loggerFactory, this.network), this.loggerFactory, newPersistentState, track);

            ISmartContractExecutionResult executionResult = vm.ExecuteMethod(
                contractCode,
                contractDetails.ContractMethodName,
                newContext,
                smartContractState.GasMeter);

            smartContractState.GasMeter.Spend(executionResult.GasConsumed);

            if (executionResult.Revert)
            {
                track.Rollback();
                return TransferResult.Failed(executionResult.Exception);
            }

            track.Commit();

            this.internalTransferList.Add(new TransferInfo
            {
                From = smartContractState.Message.ContractAddress.ToUint160(this.network),
                To = addressTo.ToUint160(this.network),
                Value = amountToTransfer
            });

            this.logger.LogTrace("(-)");

            return TransferResult.Transferred(executionResult.Return);
        }
    }
}