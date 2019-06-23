namespace MassTransit.MongoDbIntegration.Saga.Pipeline
{
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using MassTransit.Context;
    using MongoDB.Driver;
    using Util;


    public class MissingPipe<TSaga, TMessage> :
        IPipe<SagaConsumeContext<TSaga, TMessage>>
        where TSaga : class, IVersionedSaga
        where TMessage : class
    {
        readonly IMongoCollection<TSaga> _collection;
        readonly IMongoDbSagaConsumeContextFactory _mongoDbSagaConsumeContextFactory;
        readonly IPipe<SagaConsumeContext<TSaga, TMessage>> _next;

        public MissingPipe(IMongoCollection<TSaga> collection, IPipe<SagaConsumeContext<TSaga, TMessage>> next,
            IMongoDbSagaConsumeContextFactory mongoDbSagaConsumeContextFactory)
        {
            _collection = collection;
            _next = next;
            _mongoDbSagaConsumeContextFactory = mongoDbSagaConsumeContextFactory;
        }

        public void Probe(ProbeContext context)
        {
            _next.Probe(context);
        }

        public async Task Send(SagaConsumeContext<TSaga, TMessage> context)
        {
            LogContext.Debug?.Log("SAGA:{SagaType}:{CorrelationId} Added {MessageType}", TypeMetadataCache<TSaga>.ShortName,
                context.Saga.CorrelationId, TypeMetadataCache<TMessage>.ShortName);

            SagaConsumeContext<TSaga, TMessage> proxy = _mongoDbSagaConsumeContextFactory.Create(_collection, context, context.Saga, false);

            await _next.Send(proxy).ConfigureAwait(false);

            if (!proxy.IsCompleted)
                await _collection.InsertOneAsync(context.Saga).ConfigureAwait(false);
        }
    }
}
