using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using System.Collections.Generic;
using System.Numerics;

namespace OptimizedVestingSelling.Models
{
    [Function("swapExactTokensForETH")]
    public class SwapExactTokensForETHFunction : FunctionMessage
    {
        [Parameter("uint256", "amountIn", 1)]
        public BigInteger AmountIn { get; set; }

        [Parameter("uint256", "amountOutMin", 2)]
        public BigInteger AmountOutMin { get; set; }

        [Parameter("address[]", "path", 3)]
        public IList<string> Path { get; set; }

        [Parameter("address", "to", 4)]
        public string To { get; set; }

        [Parameter("uint256", "deadline", 5)]
        public BigInteger Deadline { get; set; }
    }
}
