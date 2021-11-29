namespace MassTransit.Configuration
{
    using System;
    using Courier;
    using DependencyInjection;
    using Internals;
    using Middleware;


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
            if (!_filterType.IsGenericType || !_filterType.IsGenericTypeDefinition)
                throw new ConfigurationException("The scoped filter must be a generic type definition");

            var filterType = _filterType.MakeGenericType(typeof(TLog));

            if (!filterType.HasInterface(typeof(IFilter<CompensateContext<TLog>>)))
                throw new ConfigurationException($"The scoped filter must implement {TypeCache<IFilter<CompensateContext<TLog>>>.ShortName} ");

            var scopeProvider = new CompensateActivityScopeProvider<TActivity, TLog>(_provider);

            var scopedFilterType = typeof(ScopedCompensateFilter<,,>).MakeGenericType(typeof(TActivity), typeof(TLog), filterType);

            var filter = (IFilter<CompensateContext<TLog>>)Activator.CreateInstance(scopedFilterType, scopeProvider);

            var specification = new FilterPipeSpecification<CompensateContext<TLog>>(filter);

            configurator.Log(x => x.AddPipeSpecification(specification));
        }
    }
}
