namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Saga;


    public class MessageSessionSagaRepositoryContextFactory<TSaga> :
        ISagaRepositoryContextFactory<TSaga>
        where TSaga : class, ISaga
    {
        readonly ISagaConsumeContextFactory<MessageSessionContext, TSaga> _factory;

        public MessageSessionSagaRepositoryContextFactory(ISagaConsumeContextFactory<MessageSessionContext, TSaga> factory)
        {
            _factory = factory;
        }

        public void Probe(ProbeContext context)
        {
            context.Add("persistence", "mongodb");
        }

        public async Task Send<T>(ConsumeContext<T> context, IPipe<SagaRepositoryContext<TSaga, T>> next)
            where T : class
        {
            var repositoryContext = new MessageSessionSagaRepositoryContext<TSaga, T>(context, _factory);

            await next.Send(repositoryContext).ConfigureAwait(false);
        }

        public async Task SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, IPipe<SagaRepositoryQueryContext<TSaga, T>> next)
            where T : class
        {
            throw new NotImplementedException(
                $"Query-based saga correlation is not available when using the MessageSession-based saga repository: {TypeCache<TSaga>.ShortName}");
        }

        public async Task<T> Execute<T>(Func<SagaRepositoryContext<TSaga>, Task<T>> asyncMethod, CancellationToken cancellationToken = default)
            where T : class
        {
            throw new NotImplementedException(
                $"Queries are not supported using the MessageSession-based saga repository: {TypeCache<TSaga>.ShortName}");
        }
    }
}
