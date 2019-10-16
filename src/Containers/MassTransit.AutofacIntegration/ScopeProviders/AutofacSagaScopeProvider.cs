namespace MassTransit.AutofacIntegration.ScopeProviders
{
    using System;
    using System.Collections.Generic;
    using Autofac;
    using Context;
    using GreenPipes;
    using Saga;
    using Scoping;
    using Scoping.SagaContexts;


    public class AutofacSagaScopeProvider<TSaga> :
        ISagaScopeProvider<TSaga>
        where TSaga : class, ISaga
    {
        readonly Action<ContainerBuilder, ConsumeContext> _configureScope;
        readonly string _name;
        readonly IList<Action<ConsumeContext>> _scopeActions;
        readonly ILifetimeScopeProvider _scopeProvider;

        public AutofacSagaScopeProvider(ILifetimeScopeProvider scopeProvider, string name, Action<ContainerBuilder, ConsumeContext> configureScope)
        {
            _scopeProvider = scopeProvider;
            _name = name;
            _configureScope = configureScope;
            _scopeActions = new List<Action<ConsumeContext>>();
        }

        ISagaScopeContext<T> ISagaScopeProvider<TSaga>.GetScope<T>(ConsumeContext<T> context)
        {
            if (context.TryGetPayload<ILifetimeScope>(out _))
                return new ExistingSagaScopeContext<T>(context);

            var parentLifetimeScope = _scopeProvider.GetLifetimeScope(context);

            var lifetimeScope = parentLifetimeScope.BeginLifetimeScope(_name, builder =>
            {
                builder.ConfigureScope(context);
                _configureScope?.Invoke(builder, context);
            });
            try
            {
                var proxy = new ConsumeContextScope<T>(context, lifetimeScope);

                foreach (Action<ConsumeContext> scopeAction in _scopeActions)
                    scopeAction(proxy);

                return new CreatedSagaScopeContext<ILifetimeScope, T>(lifetimeScope, proxy);
            }
            catch
            {
                lifetimeScope.Dispose();

                throw;
            }
        }

        ISagaQueryScopeContext<TSaga, T> ISagaScopeProvider<TSaga>.GetQueryScope<T>(SagaQueryConsumeContext<TSaga, T> context)
        {
            if (context.TryGetPayload<ILifetimeScope>(out _))
                return new ExistingSagaQueryScopeContext<TSaga, T>(context);

            var parentLifetimeScope = _scopeProvider.GetLifetimeScope(context);

            var lifetimeScope = parentLifetimeScope.BeginLifetimeScope(_name, builder =>
            {
                builder.ConfigureScope(context);
                _configureScope?.Invoke(builder, context);
            });
            try
            {
                var proxy = new SagaQueryConsumeContextScope<TSaga, T>(context, context.Query, lifetimeScope);

                foreach (Action<ConsumeContext> scopeAction in _scopeActions)
                    scopeAction(proxy);

                return new CreatedSagaQueryScopeContext<ILifetimeScope, TSaga, T>(lifetimeScope, proxy);
            }
            catch
            {
                lifetimeScope.Dispose();

                throw;
            }
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.Add("provider", "autofac");
            context.Add("scopeTag", _name);
        }

        public void AddScopeAction(Action<ConsumeContext> action)
        {
            _scopeActions.Add(action);
        }
    }
}
