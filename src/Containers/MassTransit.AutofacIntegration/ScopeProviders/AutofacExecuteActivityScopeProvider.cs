namespace MassTransit.AutofacIntegration.ScopeProviders
{
    using System;
    using Autofac;
    using Courier;
    using GreenPipes;
    using Scoping;
    using Scoping.CourierContexts;


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

        public IExecuteActivityScopeContext<TActivity, TArguments> GetScope(ExecuteContext<TArguments> context)
        {
            if (context.TryGetPayload<ILifetimeScope>(out var existingLifetimeScope))
            {
                var activity = existingLifetimeScope.Resolve<TActivity>(TypedParameter.From(context.Arguments));

                ExecuteActivityContext<TActivity, TArguments> activityContext = context.CreateActivityContext(activity);

                return new ExistingExecuteActivityScopeContext<TActivity, TArguments>(activityContext);
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

                ExecuteActivityContext<TActivity, TArguments> activityContext = context.CreateActivityContext(activity);

                activityContext.UpdatePayload(lifetimeScope);

                return new CreatedExecuteActivityScopeContext<ILifetimeScope, TActivity, TArguments>(lifetimeScope, activityContext);
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
