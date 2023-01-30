using OptimizedVestingSelling.Enums;

namespace OptimizedVestingSelling.Settings
{
    public class ExecutionSettings
    {
        /// <summary>
        /// Period in seconds to wait before executing the next transaction when Type == Period.
        /// </summary>
        public long Period { get; set; }

        /// <summary>
        /// Amount of tokens to sell when Type == Amount.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Slippage tolerance in percent.
        /// </summary>
        public decimal Slippage { get; set; }

        /// <summary>
        /// Address of the contract that manages the distribution
        /// </summary>
        public string ClaimAddress { get; set; }

        /// <summary>
        /// Property to determine the type of swap method to use
        /// </summary>
        public bool IsCoinToReceiveETH { get; set; }

        /// <summary>
        /// Coin address to sell
        /// </summary>
        public string CoinToSellAddress { get; set; }

        /// <summary>
        /// Coin you want to swap against the coin to sell
        /// </summary>
        public string CoinToReceiveAddress { get; set; }

        /// <summary>
        /// Type of execution settings
        /// </summary>
        public ExecutionSettingsType Type { get; set; }

        /// <summary>
        /// Address of the UniswapV2Router02 contract
        /// </summary>
        public string UniswapRouterAddress { get; set; }
    }
}
