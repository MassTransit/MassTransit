namespace MassTransit.SimpleInjectorIntegration.ScopeProviders
{
    using System;
    using System.Collections.Generic;
    using Context;
    using GreenPipes;
    using Saga;
    using Scoping;
    using Scoping.SagaContexts;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;


    public class SimpleInjectorSagaScopeProvider<TSaga> :
        ISagaScopeProvider<TSaga>
        where TSaga : class, ISaga
    {
        readonly Container _container;
        readonly IList<Action<ConsumeContext>> _scopeActions;

        public SimpleInjectorSagaScopeProvider(Container container)
        {
            _container = container;
            _scopeActions = new List<Action<ConsumeContext>>();
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "simpleInjector");
        }

        public ISagaScopeContext<T> GetScope<T>(ConsumeContext<T> context)
            where T : class
        {
            if (context.TryGetPayload<Scope>(out var existingScope))
            {
                existingScope.UpdateScope(context);

                return new ExistingSagaScopeContext<T>(context);
            }

            var scope = AsyncScopedLifestyle.BeginScope(_container);
            try
            {
                scope.UpdateScope(context);

                var proxy = new ConsumeContextScope<T>(context, scope, scope.Container);

                foreach (Action<ConsumeContext> scopeAction in _scopeActions)
                    scopeAction(proxy);

                return new CreatedSagaScopeContext<Scope, T>(scope, proxy);
            }
            catch
            {
                scope.Dispose();

                throw;
            }
        }

        public ISagaQueryScopeContext<TSaga, T> GetQueryScope<T>(SagaQueryConsumeContext<TSaga, T> context)
            where T : class
        {
            if (context.TryGetPayload<Scope>(out var existingScope))
            {
                existingScope.UpdateScope(context);

                return new ExistingSagaQueryScopeContext<TSaga, T>(context);
            }

            var scope = AsyncScopedLifestyle.BeginScope(_container);
            try
            {
                scope.UpdateScope(context);

                var proxy = new SagaQueryConsumeContextScope<TSaga, T>(context, context.Query, scope, scope.Container);

                foreach (Action<ConsumeContext> scopeAction in _scopeActions)
                    scopeAction(proxy);

                return new CreatedSagaQueryScopeContext<Scope, TSaga, T>(scope, proxy);
            }
            catch
            {
                scope.Dispose();

                throw;
            }
        }

        public void AddScopeAction(Action<ConsumeContext> action)
        {
            _scopeActions.Add(action);
        }
    }
}
