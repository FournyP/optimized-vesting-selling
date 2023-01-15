using Microsoft.Extensions.DependencyInjection;
using OptimizedVestingSelling.Enums;
using OptimizedVestingSelling.ExexutionMethods;
using System;

namespace OptimizedVestingSelling.Factories
{
    public interface IExecutionMethodFactory
    {
        public IExecutionMethod Create(ExecutionSettingsType type);
    }

    public class ExecutionMethodFactory : IExecutionMethodFactory
    {
        private readonly IServiceCollection _serviceCollection;

        public ExecutionMethodFactory(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        public IExecutionMethod Create(ExecutionSettingsType type)
        {
            return type switch {
                ExecutionSettingsType.Amount => _serviceCollection.BuildServiceProvider().GetService<IAmountExecutionMethod>(),
                ExecutionSettingsType.Period => _serviceCollection.BuildServiceProvider().GetService<IPeriodExecutionMethod>(),
                _ => throw new NotImplementedException()
            };
        }
    }
}
