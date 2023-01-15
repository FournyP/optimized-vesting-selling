using Microsoft.Extensions.Options;
using OptimizedVestingSelling.Factories;
using OptimizedVestingSelling.Settings;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OptimizedVestingSelling.ExexutionMethods
{
    public interface IAmountExecutionMethod : IExecutionMethod { }

    public class AmountExecutionMethod : IAmountExecutionMethod
    {
        private readonly IWeb3Factory _web3Factory;

        private readonly ExecutionSettings _executionSettings;

        public AmountExecutionMethod(IWeb3Factory web3Factory, IOptions<ExecutionSettings> executionSettings)
        {
            _web3Factory = web3Factory;
            _executionSettings = executionSettings.Value;
        }

        public async Task Execute()
        {
            var web3 = _web3Factory.Create();

            while (true)
            {
                var balance = await web3.Eth.GetBalance.SendRequestAsync("0x4bca9E17a8326d283d1fC68A725B46b42c2D9146");

                await Console.Out.WriteLineAsync($"Balance in Wei: {balance.Value}");

                Thread.Sleep(60 * 1000);
            }
        }
    }
}
