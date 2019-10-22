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
    /// <typeparam name="TArguments"></typeparam>
    public class AutofacExecuteActivityFactory<TActivity, TArguments> :
        IExecuteActivityFactory<TActivity, TArguments>
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly IExecuteActivityFactory<TActivity, TArguments> _factory;

        public AutofacExecuteActivityFactory(ILifetimeScope lifetimeScope, string name, Action<ContainerBuilder, ExecuteContext<TArguments>> configureScope)
        {
            var lifetimeScopeProvider = new SingleLifetimeScopeProvider(lifetimeScope);

            var executeActivityScopeProvider = new AutofacExecuteActivityScopeProvider<TActivity, TArguments>(lifetimeScopeProvider, name, configureScope);

            _factory = new ScopeExecuteActivityFactory<TActivity, TArguments>(executeActivityScopeProvider);
        }

        public Task Execute(ExecuteContext<TArguments> context, IPipe<ExecuteActivityContext<TActivity, TArguments>> next)
        {
            return _factory.Execute(context, next);
        }

        public void Probe(ProbeContext context)
        {
            _factory.Probe(context);
        }
    }
}
