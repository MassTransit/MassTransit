namespace MassTransit.WindsorIntegration.ScopeProviders
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Automatonymous;
    using Castle.MicroKernel;
    using Castle.MicroKernel.Lifestyle;
    using Context;
    using GreenPipes;
    using Saga;
    using Scoping.SagaContexts;


    public class WindsorSagaRepositoryContextFactory<TSaga> :
        ISagaRepositoryContextFactory<TSaga>
        where TSaga : class, ISaga
    {
        readonly IKernel _kernel;
        readonly IStateMachineActivityFactory _factory;

        public WindsorSagaRepositoryContextFactory(IKernel kernel)
        {
            _kernel = kernel;

            _factory = new WindsorStateMachineActivityFactory();
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.Add("provider", "windsor");
        }

        public Task<SagaRepositoryContext<TSaga, T>> CreateContext<T>(ConsumeContext<T> context, Guid? correlationId)
            where T : class
        {
            if (context.TryGetPayload<IKernel>(out var existingKernel))
            {
                existingKernel.UpdateScope(context);

                context.GetOrAddPayload(() => existingKernel.TryResolve<IStateMachineActivityFactory>() ?? _factory);

                var factory = existingKernel.Resolve<ISagaRepositoryContextFactory<TSaga>>();

                return factory.CreateContext(context, correlationId);
            }

            var scope = _kernel.CreateNewOrUseExistingMessageScope(context);

            async Task<SagaRepositoryContext<TSaga, T>> CreateMessageScope()
            {
                try
                {
                    var activityFactory = _kernel.TryResolve<IStateMachineActivityFactory>() ?? _factory;

                    var consumeContextScope = new ConsumeContextScope<T>(context, _kernel, activityFactory);

                    var factory = _kernel.Resolve<ISagaRepositoryContextFactory<TSaga>>();

                    SagaRepositoryContext<TSaga, T> repositoryContext = await factory.CreateContext(consumeContextScope, correlationId).ConfigureAwait(false);

                    return new ScopeRepositoryContext<TSaga, T>(repositoryContext, scope);
                }
                catch
                {
                    scope?.Dispose();

                    throw;
                }
            }

            return CreateMessageScope();
        }

        public async Task<SagaRepositoryContext<TSaga>> CreateContext(CancellationToken cancellationToken = default, Guid? correlationId = default)
        {
            var scope = _kernel.BeginScope();
            try
            {
                var factory = _kernel.Resolve<ISagaRepositoryContextFactory<TSaga>>();

                SagaRepositoryContext<TSaga> repositoryContext = await factory.CreateContext(cancellationToken, correlationId).ConfigureAwait(false);

                return new ScopeRepositoryContext<TSaga>(repositoryContext, scope);
            }
            catch
            {
                scope.Dispose();

                throw;
            }
        }
    }
}
