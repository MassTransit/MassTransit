namespace MassTransit.MongoDbIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using MongoDB.Driver;


    public class MongoDbSagaRepositoryContextFactory<TSaga> :
        ISagaRepositoryContextFactory<TSaga>
        where TSaga : class, ISagaVersion
    {
        readonly ISagaConsumeContextFactory<IMongoCollection<TSaga>, TSaga> _factory;
        readonly IMongoCollection<TSaga> _mongoCollection;

        public MongoDbSagaRepositoryContextFactory(IMongoCollection<TSaga> mongoCollection, ISagaConsumeContextFactory<IMongoCollection<TSaga>, TSaga> factory)
        {
            _mongoCollection = mongoCollection;
            _factory = factory;
        }

        public void Probe(ProbeContext context)
        {
            context.Add("persistence", "mongodb");
        }

        public async Task Send<T>(ConsumeContext<T> context, IPipe<SagaRepositoryContext<TSaga, T>> next)
            where T : class
        {
            var repositoryContext = new MongoDbSagaRepositoryContext<TSaga, T>(_mongoCollection, context, _factory);

            await next.Send(repositoryContext).ConfigureAwait(false);
        }

        public async Task SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, IPipe<SagaRepositoryQueryContext<TSaga, T>> next)
            where T : class
        {
            var repositoryContext = new MongoDbSagaRepositoryContext<TSaga, T>(_mongoCollection, context, _factory);

            IList<TSaga> instances = await _mongoCollection.Find(query.FilterExpression)
                .ToListAsync(repositoryContext.CancellationToken)
                .ConfigureAwait(false);

            var queryContext = new LoadedSagaRepositoryQueryContext<TSaga, T>(repositoryContext, instances);

            await next.Send(queryContext).ConfigureAwait(false);
        }

        public async Task<T> Execute<T>(Func<SagaRepositoryContext<TSaga>, Task<T>> asyncMethod, CancellationToken cancellationToken = default)
            where T : class
        {
            var repositoryContext = new MongoDbSagaRepositoryContext<TSaga>(_mongoCollection, cancellationToken);

            return await asyncMethod(repositoryContext).ConfigureAwait(false);
        }
    }
}
