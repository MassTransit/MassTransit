namespace MassTransit.WindsorIntegration.ScopeProviders
{
    using System;
    using System.Collections.Generic;
    using Castle.MicroKernel;
    using Context;
    using GreenPipes;
    using Saga;
    using Scoping;
    using Scoping.SagaContexts;


    public class WindsorSagaScopeProvider<TSaga> :
        ISagaScopeProvider<TSaga>
        where TSaga : class, ISaga
    {
        readonly IKernel _kernel;
        readonly IList<Action<ConsumeContext>> _scopeActions;

        public WindsorSagaScopeProvider(IKernel kernel)
        {
            _kernel = kernel;
            _scopeActions = new List<Action<ConsumeContext>>();
        }

        public ISagaScopeContext<T> GetScope<T>(ConsumeContext<T> context)
            where T : class
        {
            if (context.TryGetPayload<IKernel>(out var kernel))
            {
                kernel.UpdateScope(context);

                return new ExistingSagaScopeContext<T>(context);
            }

            var scope = _kernel.CreateNewOrUseExistingMessageScope(context);
            try
            {
                var proxy = new ConsumeContextScope<T>(context, _kernel);

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

        public ISagaQueryScopeContext<TSaga, T> GetQueryScope<T>(SagaQueryConsumeContext<TSaga, T> context)
            where T : class
        {
            if (context.TryGetPayload<IKernel>(out var kernel))
            {
                kernel.UpdateScope(context);

                return new ExistingSagaQueryScopeContext<TSaga, T>(context);
            }

            var scope = _kernel.CreateNewOrUseExistingMessageScope(context);
            try
            {
                var proxy = new SagaQueryConsumeContextScope<TSaga, T>(context, context.Query, _kernel);

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

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "windsor");
        }

        public void AddScopeAction(Action<ConsumeContext> action)
        {
            _scopeActions.Add(action);
        }
    }
}
