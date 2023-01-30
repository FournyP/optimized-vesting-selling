using System.Numerics;
using System.Threading.Tasks;

namespace OptimizedVestingSelling.Services
{
    public interface IEthereumUniswapGasTrackerFetcher
    {
        Task<BigInteger> Fetch(); 
    }
}
