namespace MassTransit.ExtensionsDependencyInjectionIntegration.ScopeProviders
{
    using System;
    using System.Collections.Generic;
    using Context;
    using GreenPipes;
    using Microsoft.Extensions.DependencyInjection;
    using Saga;
    using Scoping;
    using Scoping.SagaContexts;


    public class DependencyInjectionSagaScopeProvider<TSaga> :
        ISagaScopeProvider<TSaga>
        where TSaga : class, ISaga
    {
        readonly IList<Action<ConsumeContext>> _scopeActions;
        readonly IServiceProvider _serviceProvider;

        public DependencyInjectionSagaScopeProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _scopeActions = new List<Action<ConsumeContext>>();
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.Add("provider", "dependencyInjection");
        }

        ISagaScopeContext<T> ISagaScopeProvider<TSaga>.GetScope<T>(ConsumeContext<T> context)
        {
            if (context.TryGetPayload<IServiceScope>(out var existingServiceScope))
            {
                existingServiceScope.UpdateScope(context);

                return new ExistingSagaScopeContext<T>(context);
            }

            if (!context.TryGetPayload(out IServiceProvider serviceProvider))
                serviceProvider = _serviceProvider;

            var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            try
            {
                serviceScope.UpdateScope(context);

                var proxy = new ConsumeContextScope<T>(context, serviceScope, serviceScope.ServiceProvider);

                foreach (Action<ConsumeContext> scopeAction in _scopeActions)
                    scopeAction(proxy);

                return new CreatedSagaScopeContext<IServiceScope, T>(serviceScope, proxy);
            }
            catch
            {
                serviceScope.Dispose();

                throw;
            }
        }

        ISagaQueryScopeContext<TSaga, T> ISagaScopeProvider<TSaga>.GetQueryScope<T>(SagaQueryConsumeContext<TSaga, T> context)
        {
            if (context.TryGetPayload<IServiceScope>(out var existingServiceScope))
            {
                existingServiceScope.UpdateScope(context);

                return new ExistingSagaQueryScopeContext<TSaga, T>(context);
            }

            if (!context.TryGetPayload(out IServiceProvider serviceProvider))
                serviceProvider = _serviceProvider;

            var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            try
            {
                serviceScope.UpdateScope(context);

                var proxy = new SagaQueryConsumeContextScope<TSaga, T>(context, context.Query, serviceScope, serviceScope.ServiceProvider);

                foreach (Action<ConsumeContext> scopeAction in _scopeActions)
                    scopeAction(proxy);

                return new CreatedSagaQueryScopeContext<IServiceScope, TSaga, T>(serviceScope, proxy);
            }
            catch
            {
                serviceScope.Dispose();

                throw;
            }
        }

        public void AddScopeAction(Action<ConsumeContext> action)
        {
            _scopeActions.Add(action);
        }
    }
}
