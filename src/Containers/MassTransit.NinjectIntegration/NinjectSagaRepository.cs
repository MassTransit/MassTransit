namespace MassTransit.NinjectIntegration
{
    using System.Threading.Tasks;
    using GreenPipes;
    using Saga;


    public class NinjectSagaRepository<TSaga> :
        ISagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        readonly ISagaRepository<TSaga> _repository;

        public NinjectSagaRepository(ISagaRepository<TSaga> repository)
        {
            _repository = repository;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("ninject");

            _repository.Probe(scope);
        }

        public Task Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            return _repository.Send(context, policy, next);
        }

        public Task SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            return _repository.SendQuery(context, query, policy, next);
        }
    }
}
