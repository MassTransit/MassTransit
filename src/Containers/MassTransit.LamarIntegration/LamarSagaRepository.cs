namespace MassTransit.LamarIntegration
{
    using System.Threading.Tasks;
    using GreenPipes;
    using Lamar;
    using Saga;


    public class LamarSagaRepository<TSaga> : ISagaRepository<TSaga> 
        where TSaga : class, ISaga
    {
        readonly IContainer _container;
        readonly ISagaRepository<TSaga> _repository;

        public LamarSagaRepository(ISagaRepository<TSaga> repository, IContainer container)
        {
            _repository = repository;
            _container = container;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("lamar");
            _repository.Probe(scope);
        }

        async Task ISagaRepository<TSaga>.Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            using (var nestedContainer = _container.GetNestedContainer())
            {
                ConsumeContext<T> proxy = context.CreateScope(nestedContainer);

                nestedContainer.Inject(proxy);

                await _repository.Send(proxy, policy, next).ConfigureAwait(false);
            }
        }

        async Task ISagaRepository<TSaga>.SendQuery<T>(SagaQueryConsumeContext<TSaga, T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            using (var nestedContainer = _container.GetNestedContainer())
            {
                SagaQueryConsumeContext<TSaga, T> proxy = context.CreateQueryScope(nestedContainer);
                await _repository.SendQuery(proxy, policy, next).ConfigureAwait(false);
            }
        }
    }
}
