namespace MassTransit.AutofacIntegration.Filters
{
    using System;
    using ConsumeConfigurators;
    using Courier;
    using GreenPipes;
    using ScopeProviders;
    using Scoping.Filters;


    public class ScopedCompensateActivityPipeSpecificationObserver :
        IActivityConfigurationObserver
    {
        readonly Type _filterType;
        readonly ILifetimeScopeProvider _lifetimeScopeProvider;

        public ScopedCompensateActivityPipeSpecificationObserver(Type filterType, ILifetimeScopeProvider lifetimeScopeProvider)
        {
            _filterType = filterType;
            _lifetimeScopeProvider = lifetimeScopeProvider;
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
            var scopeProviderType =
                typeof(AutofacFilterContextScopeProvider<,>).MakeGenericType(_filterType.MakeGenericType(typeof(TLog)),
                    typeof(CompensateContext<TLog>));
            var scopeProvider = (IFilterContextScopeProvider<CompensateContext<TLog>>)Activator.CreateInstance(scopeProviderType, _lifetimeScopeProvider);
            var filter = new ScopedFilter<CompensateContext<TLog>>(scopeProvider);

            configurator.Log(a => a.UseFilter(filter));
        }
    }
}
