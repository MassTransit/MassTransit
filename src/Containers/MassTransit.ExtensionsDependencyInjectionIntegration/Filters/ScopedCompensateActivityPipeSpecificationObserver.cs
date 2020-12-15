namespace MassTransit.ExtensionsDependencyInjectionIntegration.Filters
{
    using System;
    using ConsumeConfigurators;
    using Courier;
    using ScopeProviders;


    public class ScopedCompensateActivityPipeSpecificationObserver :
        IActivityConfigurationObserver
    {
        readonly Type _filterType;
        readonly IServiceProvider _provider;

        public ScopedCompensateActivityPipeSpecificationObserver(Type filterType, IServiceProvider provider)
        {
            _filterType = filterType;
            _provider = provider;
        }

        public void ActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator, Uri compensateAddress)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
        }

        public void ExecuteActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
        }

        public void CompensateActivityConfigured<TActivity, TLog>(ICompensateActivityConfigurator<TActivity, TLog> configurator)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            configurator.AddScopedFilter<CompensateActivityContext<TActivity, TLog>, CompensateContext<TLog>, TLog>(_filterType, _provider);
        }
    }
}
