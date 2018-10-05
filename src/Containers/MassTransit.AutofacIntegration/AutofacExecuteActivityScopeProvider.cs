namespace MassTransit.AutofacIntegration
{
    using System;
    using Autofac;
    using Courier;
    using Courier.Hosts;
    using GreenPipes;
    using Scoping;
    using Scoping.CourierContexts;


    public class AutofacExecuteActivityScopeProvider<TActivity, TArguments> :
        IExecuteActivityScopeProvider<TActivity, TArguments>
        where TActivity : class, ExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly string _name;
        readonly Action<ContainerBuilder, ExecuteContext<TArguments>> _configurator;
        readonly ILifetimeScopeProvider _scopeProvider;

        public AutofacExecuteActivityScopeProvider(ILifetimeScopeProvider scopeProvider, string name, Action<ContainerBuilder, ExecuteContext<TArguments>> configurator)
        {
            _scopeProvider = scopeProvider;
            _name = name;
            _configurator = configurator;
        }

        public IExecuteActivityScopeContext<TActivity, TArguments> GetScope(ExecuteContext<TArguments> context)
        {
            if (context.TryGetPayload<ILifetimeScope>(out var existingLifetimeScope))
            {
                var activity = existingLifetimeScope.Resolve<TActivity>(TypedParameter.From(context.Arguments));

                ExecuteActivityContext<TActivity, TArguments> activityContext = new HostExecuteActivityContext<TActivity, TArguments>(activity, context);

                return new ExistingExecuteActivityScopeContext<TActivity, TArguments>(activityContext);
            }

            var parentLifetimeScope = _scopeProvider.GetLifetimeScope(context.ConsumeContext);

            var lifetimeScope = parentLifetimeScope.BeginLifetimeScope(_name, builder =>
            {
                builder.ConfigureScope(context.ConsumeContext);
                _configurator?.Invoke(builder, context);
            });
            try
            {
                var activity = lifetimeScope.Resolve<TActivity>(TypedParameter.From(context.Arguments));

                ExecuteActivityContext<TActivity, TArguments> activityContext = new HostExecuteActivityContext<TActivity, TArguments>(activity, context);

                var scope = lifetimeScope;
                activityContext.GetOrAddPayload(() => scope);

                return new CreatedExecuteActivityScopeContext<ILifetimeScope, TActivity, TArguments>(scope, activityContext);
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