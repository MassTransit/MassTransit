namespace MassTransit.MongoDbIntegration.Saga
{
    using System;
    using System.Threading.Tasks;
    using CollectionNameFormatters;
    using Context;
    using GreenPipes;
    using GreenPipes.Util;
    using MassTransit.Saga;
    using Metadata;
    using MongoDB.Driver;


    public class MongoDbSagaRepository<TSaga> :
        ISagaRepository<TSaga>,
        ILoadSagaRepository<TSaga>
        where TSaga : class, IVersionedSaga
    {
        readonly ISagaRepository<TSaga> _repository;
        readonly ISagaRepositoryContextFactory<TSaga> _repositoryContextFactory;

        public MongoDbSagaRepository(string connectionString, string database, string collectionName = null)
            : this(new MongoClient(connectionString).GetDatabase(database), collectionName)
        {
        }

        public MongoDbSagaRepository(MongoUrl mongoUrl, string collectionName = null)
            : this(mongoUrl.Url, mongoUrl.DatabaseName, collectionName)
        {
        }

        public MongoDbSagaRepository(IMongoDatabase mongoDatabase, string collectionName = null)
            : this(mongoDatabase, new DefaultCollectionNameFormatter(collectionName))
        {
        }

        public MongoDbSagaRepository(IMongoDatabase database, ICollectionNameFormatter collectionNameFormatter)
        {
            var mongoDbSagaConsumeContextFactory = new MongoDbSagaConsumeContextFactory<TSaga>();
            _repositoryContextFactory =
                new MongoDbSagaRepositoryContextFactory<TSaga>(database.GetCollection<TSaga>(collectionNameFormatter), mongoDbSagaConsumeContextFactory);
            _repository = new SagaRepository<TSaga>(_repositoryContextFactory);
        }

        public void Probe(ProbeContext context)
        {
            _repository.Probe(context);
        }

        public Task Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            return _repository.Send(context, policy, next);
        }

        public Task SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            return _repository.SendQuery(context, query, policy, next);
        }

        public Task<TSaga> Load(Guid correlationId)
        {
            return _repositoryContextFactory.Execute(context =>
            {
                if (context is MongoDbSagaRepositoryContext<TSaga> mongoDbSagaRepositoryContext)
                    return mongoDbSagaRepositoryContext.Load(correlationId);

                return TaskUtil.Faulted<TSaga>(new NotSupportedException($"{nameof(Load)} is not supported for {TypeMetadataCache<TSaga>.ShortName}"));
            });
        }
    }
}
