using Microsoft.Extensions.Options;
using OptimizedVestingSelling.Factories;
using OptimizedVestingSelling.Settings;
using System.Threading.Tasks;

namespace ApplicationTemplate
{
    public class App
    {
        private readonly ExecutionSettings _executionSettings;

        private readonly IExecutionMethodFactory _executionMethodFactory;

        public App(
            IOptions<ExecutionSettings> executionSettings, 
            IExecutionMethodFactory executionMethodFactory)
        {
            _executionSettings = executionSettings.Value;
            _executionMethodFactory = executionMethodFactory;
        }

        // Async application starting point
        public async Task Run()
        {
            var executionMethod = _executionMethodFactory.Create(_executionSettings.Type);

            await executionMethod.Execute();
        }
    }
}
