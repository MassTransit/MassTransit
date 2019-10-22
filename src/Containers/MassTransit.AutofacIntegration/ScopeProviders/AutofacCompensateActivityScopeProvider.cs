namespace MassTransit.AutofacIntegration.ScopeProviders
{
    using System;
    using Autofac;
    using Courier;
    using GreenPipes;
    using Scoping;
    using Scoping.CourierContexts;


    public class AutofacCompensateActivityScopeProvider<TActivity, TLog> :
        ICompensateActivityScopeProvider<TActivity, TLog>
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
        readonly Action<ContainerBuilder, CompensateContext<TLog>> _configureScope;
        readonly string _name;
        readonly ILifetimeScopeProvider _scopeProvider;

        public AutofacCompensateActivityScopeProvider(ILifetimeScopeProvider scopeProvider, string name, Action<ContainerBuilder, CompensateContext<TLog>>
            configureScope = null)
        {
            _scopeProvider = scopeProvider;
            _name = name;
            _configureScope = configureScope;
        }

        public ICompensateActivityScopeContext<TActivity, TLog> GetScope(CompensateContext<TLog> context)
        {
            if (context.TryGetPayload<ILifetimeScope>(out var existingLifetimeScope))
            {
                var activity = existingLifetimeScope.Resolve<TActivity>(TypedParameter.From(context.Log));

                CompensateActivityContext<TActivity, TLog> activityContext = context.CreateActivityContext(activity);

                return new ExistingCompensateActivityScopeContext<TActivity, TLog>(activityContext);
            }

            var parentLifetimeScope = _scopeProvider.GetLifetimeScope(context);

            var lifetimeScope = parentLifetimeScope.BeginLifetimeScope(_name, builder =>
            {
                builder.ConfigureScope(context);
                _configureScope?.Invoke(builder, context);
            });

            try
            {
                var activity = lifetimeScope.Resolve<TActivity>(TypedParameter.From(context.Log));

                CompensateActivityContext<TActivity, TLog> activityContext = context.CreateActivityContext(activity);

                activityContext.UpdatePayload(lifetimeScope);

                return new CreatedCompensateActivityScopeContext<ILifetimeScope, TActivity, TLog>(lifetimeScope, activityContext);
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
