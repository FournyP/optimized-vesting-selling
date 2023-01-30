using Microsoft.Extensions.Options;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Util;
using Nethereum.Web3;
using OptimizedVestingSelling.Factories;
using OptimizedVestingSelling.Models;
using OptimizedVestingSelling.Services;
using OptimizedVestingSelling.Settings;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace OptimizedVestingSelling.ExexutionMethods
{
    public interface IAmountExecutionMethod : IExecutionMethod { }

    public class AmountExecutionMethod : IAmountExecutionMethod
    {
        private readonly IWeb3Factory _web3Factory;

        private readonly WalletSettings _walletSettings;

        private readonly ExecutionSettings _executionSettings;

        private readonly IEthereumUniswapGasTrackerFetcher _ethereumUniswapGasTrackerFetcher;

        public AmountExecutionMethod(
            IWeb3Factory web3Factory, 
            IOptions<WalletSettings> walletSettings, 
            IOptions<ExecutionSettings> executionSettings,
            IEthereumUniswapGasTrackerFetcher ethereumUniswapGasTrackerFetcher)
        {
            _web3Factory = web3Factory;
            _walletSettings = walletSettings.Value;
            _executionSettings = executionSettings.Value;
            _ethereumUniswapGasTrackerFetcher = ethereumUniswapGasTrackerFetcher;
        }

        public async Task Execute()
        {
            var web3 = _web3Factory.Create();

            while (true)
            {
                if (!_executionSettings.IsCoinToReceiveETH)
                {
                    throw new NotImplementedException();
                }

                var totalToSell = await GetPotentialAmountToSell(web3);
                var amountOutWei = await GetPotentielAmountOut(web3, totalToSell);

                var gasPrice = await web3.Eth.GasPrice.SendRequestAsync();

                var (claimFunction, claimHandler) = await PrepareClaimFunction(web3, gasPrice);

                var amountOutMin = new BigInteger(((decimal)amountOutWei) * (1 - _executionSettings.Slippage));

                var (swapExactTokensForETHFunction, swapExactTokensForETHHandler) = PrepareSwapExactTokensForETHFunction(web3, gasPrice, totalToSell, amountOutMin);

                var swapEstimatedFees = await _ethereumUniswapGasTrackerFetcher.Fetch();
                var claimEstimatedFees = BigInteger.Multiply(claimFunction.Gas.Value, claimFunction.GasPrice.Value);

                var totalFeesEstimated = BigInteger.Add(swapEstimatedFees, claimEstimatedFees);
                var amountSlippage = new BigInteger(((decimal)amountOutMin) / _executionSettings.Amount);

                if (amountSlippage > totalFeesEstimated)
                {
                    var receipt = await claimHandler.SendRequestAndWaitForReceiptAsync(_executionSettings.ClaimAddress, claimFunction);

                    if (receipt.HasErrors().HasValue && receipt.HasErrors().Value)
                    {
                        Log.Error("An error has been receive when execute claim");
                    }
                    else
                    {
                        receipt = await swapExactTokensForETHHandler.SendRequestAndWaitForReceiptAsync(_executionSettings.UniswapRouterAddress, swapExactTokensForETHFunction);

                        if (receipt.HasErrors().HasValue && receipt.HasErrors().Value)
                        {
                            Log.Error("An error has been receive when execute swap");
                        }
                    }
                }

                // Wait a minute
                Thread.Sleep(1 * 60 * 1000);
            }
        }

        /// <summary>
        /// Method to retrieve the amount of coin to sell
        /// </summary>
        /// <param name="web3"></param>
        /// <returns></returns>
        private async Task<BigInteger> GetPotentialAmountToSell(IWeb3 web3)
        {
            var getAmountToClaimFunction = new GetAmountToClaimFunction
            {
                Address = _walletSettings.PublicAddress
            };

            var amountToClaimWei = await web3.Eth.GetContractQueryHandler<GetAmountToClaimFunction>()
                .QueryAsync<BigInteger>(_executionSettings.ClaimAddress, getAmountToClaimFunction);

            var balanceOfFunction = new BalanceOfFunction
            {
                Owner = _walletSettings.PublicAddress
            };

            var balanceOfWei = await web3.Eth.GetContractQueryHandler<BalanceOfFunction>()
                .QueryAsync<BigInteger>(_executionSettings.CoinToSellAddress, balanceOfFunction);

            return BigInteger.Add(amountToClaimWei, balanceOfWei);
        }

        /// <summary>
        /// Method to retrieve the amount of coin to receive
        /// </summary>
        /// <param name="web3"></param>
        /// <param name="totalToSell"></param>
        /// <returns></returns>
        private async Task<BigInteger> GetPotentielAmountOut(IWeb3 web3, BigInteger totalToSell)
        {
            var getAmountsOutFunction = new GetAmountsOutFunction
            {
                AmountIn = totalToSell,
                Path = new List<string>() { _executionSettings.CoinToSellAddress, _executionSettings.CoinToReceiveAddress }
            };

            var amountsOutWei = await web3.Eth.GetContractQueryHandler<GetAmountsOutFunction>()
                .QueryAsync<List<BigInteger>>(_executionSettings.UniswapRouterAddress, getAmountsOutFunction);

            return amountsOutWei.Last();
        }

        /// <summary>
        /// Method to prepare the claim function
        /// </summary>
        /// <param name="web3"></param>
        /// <param name="gasPrice"></param>
        /// <returns></returns>
        private async Task<Tuple<ClaimFunction, IContractTransactionHandler<ClaimFunction>>> PrepareClaimFunction(IWeb3 web3, BigInteger gasPrice)
        {
            var claimFunction = new ClaimFunction
            {
                GasPrice = gasPrice
            };
            var claimHandler = web3.Eth.GetContractTransactionHandler<ClaimFunction>();

            var claimEstimedGasFees = await claimHandler.EstimateGasAsync(_executionSettings.ClaimAddress, claimFunction);
            claimFunction.Gas = claimEstimedGasFees;

            return new Tuple<ClaimFunction, IContractTransactionHandler<ClaimFunction>>(claimFunction, claimHandler);
        }

        /// <summary>
        /// Method to prepare the swap function
        /// </summary>
        /// <param name="web3"></param>
        /// <param name="gasPrice"></param>
        /// <param name="totalToSell"></param>
        /// <param name="amountOutMin"></param>
        /// <returns></returns>
        private Tuple<SwapExactTokensForETHFunction, IContractTransactionHandler<SwapExactTokensForETHFunction>> PrepareSwapExactTokensForETHFunction(IWeb3 web3, BigInteger gasPrice, BigInteger totalToSell, BigInteger amountOutMin)
        {
            var swapExactTokensForETHFunction = new SwapExactTokensForETHFunction
            {
                GasPrice = gasPrice,
                AmountIn = totalToSell,
                AmountOutMin = amountOutMin,
                To = _walletSettings.PublicAddress,
                Deadline = DateTimeOffset.Now.AddMinutes(1).ToUnixTimeMilliseconds(),
                Path = new List<string>() { _executionSettings.CoinToSellAddress, _executionSettings.CoinToReceiveAddress }
            };
            var swapExactTokensForETHHandler = web3.Eth.GetContractTransactionHandler<SwapExactTokensForETHFunction>();

            // Cannot estimate fees because dind't have enough token to sell (some of them are not already claim)

            return new Tuple<SwapExactTokensForETHFunction, IContractTransactionHandler<SwapExactTokensForETHFunction>>(swapExactTokensForETHFunction, swapExactTokensForETHHandler);
        }
    }
}
