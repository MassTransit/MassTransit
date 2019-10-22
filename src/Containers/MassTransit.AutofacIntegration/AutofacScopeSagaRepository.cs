namespace MassTransit.AutofacIntegration
{
    using System;
    using System.Threading.Tasks;
    using Autofac;
    using GreenPipes;
    using Saga;
    using ScopeProviders;
    using Scoping;


    public class AutofacScopeSagaRepository<TSaga, TId> :
        ISagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        readonly ISagaRepository<TSaga> _repository;

        public AutofacScopeSagaRepository(ISagaRepository<TSaga> repository, ILifetimeScopeRegistry<TId> registry, string name,
            Action<ContainerBuilder, ConsumeContext> configureScope)
        {
            ISagaScopeProvider<TSaga> scopeProvider =
                new AutofacSagaScopeProvider<TSaga>(new RegistryLifetimeScopeProvider<TId>(registry), name, configureScope);
            _repository = new ScopeSagaRepository<TSaga>(repository, scopeProvider);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _repository.Probe(context);
        }

        Task ISagaRepository<TSaga>.Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            return _repository.Send(context, policy, next);
        }

        Task ISagaRepository<TSaga>.SendQuery<T>(SagaQueryConsumeContext<TSaga, T> context, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            return _repository.SendQuery(context, policy, next);
        }
    }
}
