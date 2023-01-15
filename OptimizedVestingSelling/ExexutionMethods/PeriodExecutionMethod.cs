using Microsoft.Extensions.Options;
using OptimizedVestingSelling.Factories;
using OptimizedVestingSelling.Settings;
using System;
using System.Threading.Tasks;

namespace OptimizedVestingSelling.ExexutionMethods
{
    public interface IPeriodExecutionMethod : IExecutionMethod { }

    public class PeriodExecutionMethod : IPeriodExecutionMethod
    {
        private readonly IWeb3Factory _web3Factory;

        private readonly ExecutionSettings _executionSettings;

        public PeriodExecutionMethod(IWeb3Factory web3Factory, IOptions<ExecutionSettings> executionSettings)
        {
            _web3Factory = web3Factory;
            _executionSettings = executionSettings.Value;
        }
        
        public Task Execute()
        {
            throw new NotImplementedException();
        }
    }
}
