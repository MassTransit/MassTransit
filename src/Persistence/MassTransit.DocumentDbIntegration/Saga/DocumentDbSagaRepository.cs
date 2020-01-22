namespace MassTransit.DocumentDbIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using MassTransit.Saga;
    using Microsoft.Azure.Documents;
    using Newtonsoft.Json;


    public class DocumentDbSagaRepository<TSaga> :
        ISagaRepository<TSaga>,
        IQuerySagaRepository<TSaga>
        where TSaga : class, IVersionedSaga
    {
        const string DefaultCollectionName = "sagas";

        readonly SagaRepository<TSaga> _repository;

        public DocumentDbSagaRepository(IDocumentClient client, string databaseName, JsonSerializerSettings serializerSettings = null)
            : this(client, databaseName, DefaultCollectionName, serializerSettings)
        {
        }

        public DocumentDbSagaRepository(IDocumentClient client, string databaseName, string collectionName,
            JsonSerializerSettings serializerSettings = null)
        {
            if (string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentNullException(nameof(collectionName));

            if (collectionName.Length > 120)
                throw new ArgumentException("Collection names must be no longer than 120 characters", nameof(collectionName));

            var databaseContext = new DocumentDbDatabaseContext<TSaga>(client, databaseName, collectionName ?? DefaultCollectionName, serializerSettings);

            var consumeContextFactory = new DocumentDbSagaConsumeContextFactory<TSaga>();

            var repositoryFactory = new DocumentDbSagaRepositoryContextFactory<TSaga>(databaseContext, consumeContextFactory);

            _repository = new SagaRepository<TSaga>(repositoryFactory);
        }

        public Task<IEnumerable<Guid>> Find(ISagaQuery<TSaga> query)
        {
            return _repository.Find(query);
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
    }
}
