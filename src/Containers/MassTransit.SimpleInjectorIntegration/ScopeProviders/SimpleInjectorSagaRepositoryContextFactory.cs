namespace MassTransit.SimpleInjectorIntegration.ScopeProviders
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Automatonymous;
    using Context;
    using GreenPipes;
    using Saga;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;


    public class SimpleInjectorSagaRepositoryContextFactory<TSaga> :
        ISagaRepositoryContextFactory<TSaga>
        where TSaga : class, ISaga
    {
        readonly Container _container;

        public SimpleInjectorSagaRepositoryContextFactory(Container container)
        {
            _container = container;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.Add("provider", "simpleInjector");
        }

        public Task Send<T>(ConsumeContext<T> context, IPipe<SagaRepositoryContext<TSaga, T>> next)
            where T : class
        {
            return Send(context, (consumeContext, factory) => factory.Send(consumeContext, next));
        }

        public Task SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, IPipe<SagaRepositoryQueryContext<TSaga, T>> next)
            where T : class
        {
            return Send(context, (consumeContext, factory) => factory.SendQuery(consumeContext, query, next));
        }

        public async Task<T> Execute<T>(Func<SagaRepositoryContext<TSaga>, Task<T>> asyncMethod, CancellationToken cancellationToken)
            where T : class
        {
            using var scope = AsyncScopedLifestyle.BeginScope(_container);

            var factory = scope.GetInstance<ISagaRepositoryContextFactory<TSaga>>();

            return await factory.Execute(asyncMethod, cancellationToken).ConfigureAwait(false);
        }

        Task Send<T>(ConsumeContext<T> context, Func<ConsumeContext<T>, ISagaRepositoryContextFactory<TSaga>, Task> send)
            where T : class
        {
            if (context.TryGetPayload<Scope>(out var existingScope))
            {
                existingScope.UpdateScope(context);

                context.GetOrAddPayload(() => existingScope.TryGetInstance<IStateMachineActivityFactory>()
                    ?? SimpleInjectorStateMachineActivityFactory.Instance);

                var factory = existingScope.GetInstance<ISagaRepositoryContextFactory<TSaga>>();

                return send(context, factory);
            }

            var scope = AsyncScopedLifestyle.BeginScope(_container);

            async Task CreateNestedContainer()
            {
                try
                {
                    scope.UpdateScope(context);

                    var activityFactory = scope.TryGetInstance<IStateMachineActivityFactory>()
                        ?? SimpleInjectorStateMachineActivityFactory.Instance;

                    var consumeContextScope = new ConsumeContextScope<T>(context, scope, activityFactory);

                    var factory = scope.GetInstance<ISagaRepositoryContextFactory<TSaga>>();

                    await send(consumeContextScope, factory).ConfigureAwait(false);
                }
                finally
                {
                    scope.Dispose();
                }
            }

            return CreateNestedContainer();
        }
    }
}
