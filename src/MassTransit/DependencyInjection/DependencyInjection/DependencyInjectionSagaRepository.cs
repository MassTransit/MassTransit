namespace MassTransit.DependencyInjection
{
    using System;
    using System.Threading.Tasks;
    using Middleware;
    using Saga;


    public class DependencyInjectionSagaRepository<TSaga> :
        ISagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        readonly ISagaRepositoryContextFactory<TSaga> _repositoryContextFactory;

        public DependencyInjectionSagaRepository(IRegistrationContext context)
            : this(new DependencyInjectionSagaRepositoryContextFactory<TSaga>(context))
        {
        }

        public DependencyInjectionSagaRepository(IServiceProvider serviceProvider, ISetScopedConsumeContext setter)
            : this(new DependencyInjectionSagaRepositoryContextFactory<TSaga>(serviceProvider, setter))
        {
        }

        DependencyInjectionSagaRepository(ISagaRepositoryContextFactory<TSaga> repositoryContextFactory)
        {
            _repositoryContextFactory = repositoryContextFactory;
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("dependencyInjectionSagaRepository");

            _repositoryContextFactory.Probe(scope);
        }

        public Task Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            var correlationId = context.CorrelationId ??
                throw new SagaException("The CorrelationId was not specified", typeof(TSaga), typeof(T));

            return _repositoryContextFactory.Send(context, new SendSagaPipe<TSaga, T>(policy, next, correlationId));
        }

        public Task SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            return _repositoryContextFactory.SendQuery(context, query, new SendQuerySagaPipe<TSaga, T>(policy, next));
        }
    }
}
