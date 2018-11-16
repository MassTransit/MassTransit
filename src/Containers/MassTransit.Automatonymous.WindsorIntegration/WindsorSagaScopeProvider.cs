namespace MassTransit.AutomatonymousWindsorIntegration
{
    using Castle.MicroKernel;
    using Castle.MicroKernel.Lifestyle;
    using Castle.MicroKernel.Registration;
    using Context;
    using GreenPipes;
    using GreenPipes.Payloads;
    using Saga;
    using Scoping;
    using Scoping.SagaContexts;
    using System;
    using System.Collections.Generic;


    public class WindsorSagaScopeProvider<TSaga> :
        ISagaScopeProvider<TSaga>
        where TSaga : class, ISaga
    {
        readonly IKernel _container;
        readonly IList<Action<ConsumeContext>> _scopeActions;

        public WindsorSagaScopeProvider(IKernel container)
        {
            _container = container;
            _scopeActions = new List<Action<ConsumeContext>>();
        }

        public void AddScopeAction(Action<ConsumeContext> action)
        {
            _scopeActions.Add(action);
        }

        public ISagaQueryScopeContext<TSaga, T> GetQueryScope<T>(SagaQueryConsumeContext<TSaga, T> context) where T : class
        {
            if (context.TryGetPayload<IKernel>(out _))
                return new ExistingSagaQueryScopeContext<TSaga, T>(context);

            var scope = _container.BeginScope();

            _container.Register(Component.For<ConsumeContext, ConsumeContext<T>>().Instance(context));

            try
            {
                var proxy = new SagaQueryConsumeContextProxy<TSaga, T>(context, new PayloadCacheScope(context), context.Query);

                var consumerContainer = _container;
                proxy.GetOrAddPayload(() => consumerContainer);
                foreach (Action<ConsumeContext> scopeAction in _scopeActions)
                    scopeAction(proxy);

                return new CreatedSagaQueryScopeContext<IDisposable, TSaga, T>(scope, proxy);
            }
            catch
            {
                scope.Dispose();

                throw;
            }

        }

        public ISagaScopeContext<T> GetScope<T>(ConsumeContext<T> context) where T : class
        {
            if (context.TryGetPayload<IKernel>(out _))
                return new ExistingSagaScopeContext<T>(context);

            var scope = _container.BeginScope();

            _container.Register(Component.For<ConsumeContext, ConsumeContext<T>>().Instance(context));

            try
            {
                var proxy = new ConsumeContextProxy<T>(context, new PayloadCacheScope(context));

                var consumerContainer = _container;

                proxy.GetOrAddPayload(() => consumerContainer);
                foreach (Action<ConsumeContext> scopeAction in _scopeActions)
                    scopeAction(proxy);

                return new CreatedSagaScopeContext<IDisposable, T>(scope, proxy);
            }
            catch
            {
                scope.Dispose();

                throw;
            }

        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "windsor");
        }
    }
}
