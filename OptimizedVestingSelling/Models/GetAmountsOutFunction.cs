using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using System.Collections.Generic;
using System.Numerics;

namespace OptimizedVestingSelling.Models
{
    [Function("getAmountsOut", "uint[]")]
    public class GetAmountsOutFunction : FunctionMessage
    {
        [Parameter("uint256", "amountIn", 1)]
        public BigInteger AmountIn { get; set; }

        [Parameter("address[]", "path", 2)]
        public IList<string> Path { get; set; }
    }
}
