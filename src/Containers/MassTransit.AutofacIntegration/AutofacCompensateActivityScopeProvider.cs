namespace MassTransit.AutofacIntegration
{
    using System;
    using Autofac;
    using Courier;
    using Courier.Hosts;
    using GreenPipes;
    using Scoping;
    using Scoping.CourierContexts;


    public class AutofacCompensateActivityScopeProvider<TActivity, TLog> :
        ICompensateActivityScopeProvider<TActivity, TLog>
        where TActivity : class, CompensateActivity<TLog>
        where TLog : class
    {
        readonly string _name;
        readonly Action<ContainerBuilder, CompensateContext<TLog>> _configurator;
        readonly ILifetimeScopeProvider _scopeProvider;

        public AutofacCompensateActivityScopeProvider(ILifetimeScopeProvider scopeProvider, string name, Action<ContainerBuilder, CompensateContext<TLog>> configurator)
        {
            _scopeProvider = scopeProvider;
            _name = name;
            _configurator = configurator;
        }

        public ICompensateActivityScopeContext<TActivity, TLog> GetScope(CompensateContext<TLog> context)
        {
            if (context.TryGetPayload<ILifetimeScope>(out var existingLifetimeScope))
            {
                var activity = existingLifetimeScope.Resolve<TActivity>(TypedParameter.From(context.Log));

                CompensateActivityContext<TActivity, TLog> activityContext = new HostCompensateActivityContext<TActivity, TLog>(activity, context);

                return new ExistingCompensateActivityScopeContext<TActivity, TLog>(activityContext);
            }

            var parentLifetimeScope = _scopeProvider.GetLifetimeScope(context.ConsumeContext);

            var lifetimeScope = parentLifetimeScope.BeginLifetimeScope(_name, builder =>
            {
                builder.ConfigureScope(context.ConsumeContext);
                _configurator?.Invoke(builder, context);
            });
            try
            {
                var activity = lifetimeScope.Resolve<TActivity>(TypedParameter.From(context.Log));

                CompensateActivityContext<TActivity, TLog> activityContext = new HostCompensateActivityContext<TActivity, TLog>(activity, context);

                var scope = lifetimeScope;
                activityContext.GetOrAddPayload(() => scope);

                return new CreatedCompensateActivityScopeContext<ILifetimeScope, TActivity, TLog>(scope, activityContext);
            }
            catch
            {
                lifetimeScope.Dispose();

                throw;
            }
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "autofac");
            context.Add("scopeTag", _name);
        }
    }
}