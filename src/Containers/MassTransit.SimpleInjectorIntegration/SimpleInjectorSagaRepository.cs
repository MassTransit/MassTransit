namespace MassTransit.SimpleInjectorIntegration
{
    using System.Threading.Tasks;

    using SimpleInjector;

    using MassTransit.Pipeline;
    using MassTransit.Saga;

    using SimpleInjector.Extensions.ExecutionContextScoping;


    public class SimpleInjectorSagaRepository<TSaga> :
        ISagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        readonly Container _container;
        readonly ISagaRepository<TSaga> _repository;

        public SimpleInjectorSagaRepository(ISagaRepository<TSaga> repository, Container container)
        {
            this._repository = repository;
            this._container = container;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateScope("simpleinjector");

            this._repository.Probe(scope);
        }

        public async Task Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next) where T : class
        {
            using (this._container.BeginExecutionContextScope())
            {
                await this._repository.Send(context, policy, next);
            }
        }

        public async Task SendQuery<T>(SagaQueryConsumeContext<TSaga, T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            using (this._container.BeginExecutionContextScope())
            {
                await this._repository.SendQuery(context, policy, next);
            }
        }
    }
}