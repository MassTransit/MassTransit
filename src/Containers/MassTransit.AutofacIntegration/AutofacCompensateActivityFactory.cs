namespace MassTransit.AutofacIntegration
{
    using System;
    using System.Threading.Tasks;
    using Autofac;
    using Courier;
    using GreenPipes;
    using ScopeProviders;
    using Scoping;


    /// <summary>
    /// A factory to create an activity from Autofac, that manages the lifetime scope of the activity
    /// </summary>
    /// <typeparam name="TActivity"></typeparam>
    /// <typeparam name="TLog"></typeparam>
    public class AutofacCompensateActivityFactory<TActivity, TLog> :
        ICompensateActivityFactory<TActivity, TLog>
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
        readonly ICompensateActivityFactory<TActivity, TLog> _factory;

        public AutofacCompensateActivityFactory(ILifetimeScope lifetimeScope, string name, Action<ContainerBuilder, CompensateContext<TLog>> configureScope)
        {
            var lifetimeScopeProvider = new SingleLifetimeScopeProvider(lifetimeScope);

            var compensateActivityScopeProvider = new AutofacCompensateActivityScopeProvider<TActivity, TLog>(lifetimeScopeProvider, name, configureScope);

            _factory = new ScopeCompensateActivityFactory<TActivity, TLog>(compensateActivityScopeProvider);
        }

        public Task Compensate(CompensateContext<TLog> context, IPipe<CompensateActivityContext<TActivity, TLog>> next)
        {
            return _factory.Compensate(context, next);
        }

        public void Probe(ProbeContext context)
        {
            _factory.Probe(context);
        }
    }
}
