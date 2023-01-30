using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace OptimizedVestingSelling.Models
{
    [Function("getAmountToClaim", "uint256")]
    public class GetAmountToClaimFunction : FunctionMessage
    {
        [Parameter("address")]
        public string Address { get; set; }
    }
}
