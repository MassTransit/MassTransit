namespace MassTransit.AutofacIntegration.ScopeProviders
{
    using System;
    using System.Threading.Tasks;
    using Autofac;
    using Courier;
    using GreenPipes;
    using Scoping;
    using Scoping.CourierContexts;
    using Util;


    public class AutofacExecuteActivityScopeProvider<TActivity, TArguments> :
        IExecuteActivityScopeProvider<TActivity, TArguments>
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly Action<ContainerBuilder, ExecuteContext<TArguments>> _configureScope;
        readonly string _name;
        readonly ILifetimeScopeProvider _scopeProvider;

        public AutofacExecuteActivityScopeProvider(ILifetimeScopeProvider scopeProvider, string name, Action<ContainerBuilder, ExecuteContext<TArguments>>
            configureScope = null)
        {
            _scopeProvider = scopeProvider;
            _name = name;
            _configureScope = configureScope;
        }

        public ValueTask<IExecuteActivityScopeContext<TActivity, TArguments>> GetScope(ExecuteContext<TArguments> context)
        {
            if (context.TryGetPayload<ILifetimeScope>(out var existingLifetimeScope))
            {
                var activity = existingLifetimeScope.Resolve<TActivity>(TypedParameter.From(context.Arguments));

                ExecuteActivityContext<TActivity, TArguments> activityContext = context.CreateActivityContext(activity);

                return new ValueTask<IExecuteActivityScopeContext<TActivity, TArguments>>(
                    new ExistingExecuteActivityScopeContext<TActivity, TArguments>(activityContext));
            }

            var parentLifetimeScope = _scopeProvider.GetLifetimeScope(context);

            var lifetimeScope = parentLifetimeScope.BeginLifetimeScope(_name, builder =>
            {
                builder.ConfigureScope(context);
                _configureScope?.Invoke(builder, context);
            });

            try
            {
                var activity = lifetimeScope.Resolve<TActivity>(TypedParameter.From(context.Arguments));

                var executeContext = lifetimeScope.Resolve<ExecuteContext<TArguments>>();

                ExecuteActivityContext<TActivity, TArguments> activityContext = executeContext.CreateActivityContext(activity);

                return new ValueTask<IExecuteActivityScopeContext<TActivity, TArguments>>(
                    new CreatedExecuteActivityScopeContext<ILifetimeScope, TActivity, TArguments>(lifetimeScope, activityContext));
            }
            catch (Exception ex)
            {
                return ex.DisposeAsync<IExecuteActivityScopeContext<TActivity, TArguments>>(() => lifetimeScope.DisposeAsync());
            }
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "autofac");
            context.Add("scopeTag", _name);
        }
    }
}
