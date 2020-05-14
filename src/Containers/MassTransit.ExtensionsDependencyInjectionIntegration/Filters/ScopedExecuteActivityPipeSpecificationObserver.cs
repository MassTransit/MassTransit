namespace MassTransit.ExtensionsDependencyInjectionIntegration.Filters
{
    using System;
    using ConsumeConfigurators;
    using Courier;
    using GreenPipes;
    using ScopeProviders;
    using Scoping.Filters;


    public class ScopedExecuteActivityPipeSpecificationObserver :
        IActivityConfigurationObserver
    {
        readonly Type _filterType;
        readonly IServiceProvider _serviceProvider;

        public ScopedExecuteActivityPipeSpecificationObserver(Type filterType, IServiceProvider serviceProvider)
        {
            _filterType = filterType;
            _serviceProvider = serviceProvider;
        }

        public void ActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator, Uri compensateAddress)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            ExecuteActivityConfigured(configurator);
        }

        public void ExecuteActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            var scopeProviderType =
                typeof(DependencyInjectionFilterContextScopeProvider<,>).MakeGenericType(_filterType.MakeGenericType(typeof(TArguments)),
                    typeof(ExecuteContext<TArguments>));
            var scopeProvider = (IFilterContextScopeProvider<ExecuteContext<TArguments>>)Activator.CreateInstance(scopeProviderType, _serviceProvider);
            var filter = new ScopedFilter<ExecuteContext<TArguments>>(scopeProvider);

            configurator.Arguments(a => a.UseFilter(filter));
        }

        public void CompensateActivityConfigured<TActivity, TLog>(ICompensateActivityConfigurator<TActivity, TLog> configurator)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
        }
    }
}
