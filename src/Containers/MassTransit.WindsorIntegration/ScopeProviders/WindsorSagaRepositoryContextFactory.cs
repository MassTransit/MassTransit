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


    public class WindsorSagaRepositoryContextFactory<TSaga> :
        ISagaRepositoryContextFactory<TSaga>
        where TSaga : class, ISaga
    {
        readonly IKernel _kernel;

        public WindsorSagaRepositoryContextFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.Add("provider", "windsor");
        }

        public Task Send<T>(ConsumeContext<T> context, IPipe<SagaRepositoryContext<TSaga, T>> next)
            where T : class
        {
            if (context.TryGetPayload<IKernel>(out var existingKernel))
            {
                existingKernel.UpdateScope(context);

                context.GetOrAddPayload(() => existingKernel.TryResolve<IStateMachineActivityFactory>() ?? WindsorStateMachineActivityFactory.Instance);

                var factory = existingKernel.Resolve<ISagaRepositoryContextFactory<TSaga>>();

                return factory.Send(context, next);
            }

            var scope = _kernel.CreateNewOrUseExistingMessageScope(context);

            async Task CreateMessageScope()
            {
                try
                {
                    var activityFactory = _kernel.TryResolve<IStateMachineActivityFactory>() ?? WindsorStateMachineActivityFactory.Instance;

                    var consumeContextScope = new ConsumeContextScope<T>(context, _kernel, activityFactory);

                    var factory = _kernel.Resolve<ISagaRepositoryContextFactory<TSaga>>();

                    await factory.Send(consumeContextScope, next).ConfigureAwait(false);
                }
                finally
                {
                    scope.Dispose();
                }
            }

            return CreateMessageScope();
        }

        public async Task<T> Execute<T>(Func<SagaRepositoryContext<TSaga>, Task<T>> asyncMethod, CancellationToken cancellationToken)
            where T : class
        {
            using var scope = _kernel.BeginScope();

            var factory = _kernel.Resolve<ISagaRepositoryContextFactory<TSaga>>();

            return await factory.Execute(asyncMethod, cancellationToken).ConfigureAwait(false);
        }
    }
}
