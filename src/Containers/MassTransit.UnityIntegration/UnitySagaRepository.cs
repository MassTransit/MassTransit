namespace MassTransit.UnityIntegration
{
    using System.Threading.Tasks;
    using GreenPipes;
    using Saga;
    using Scoping;
    using Unity;


    public class UnitySagaRepository<TSaga> :
        ISagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        readonly ISagaRepository<TSaga> _repository;

        public UnitySagaRepository(ISagaRepository<TSaga> repository, IUnityContainer container)
        {
            _repository = new ScopeSagaRepository<TSaga>(repository, new UnitySagaScopeProvider<TSaga>(container));
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _repository.Probe(context);
        }

        Task ISagaRepository<TSaga>.Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            return _repository.Send(context, policy, next);
        }

        Task ISagaRepository<TSaga>.SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            return _repository.SendQuery(context, query, policy, next);
        }
    }
}
