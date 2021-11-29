namespace MassTransit.AzureTable.Saga
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using Microsoft.Azure.Cosmos.Table;


    public class AzureTableSagaRepositoryContextFactory<TSaga> :
        ISagaRepositoryContextFactory<TSaga>
        where TSaga : class, ISaga
    {
        readonly ICloudTableProvider<TSaga> _cloudTableProvider;
        readonly ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> _factory;
        readonly ISagaKeyFormatter<TSaga> _keyFormatter;

        public AzureTableSagaRepositoryContextFactory(ICloudTableProvider<TSaga> cloudTableProvider,
            ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> factory,
            ISagaKeyFormatter<TSaga> keyFormatter)
        {
            _cloudTableProvider = cloudTableProvider;
            _factory = factory;
            _keyFormatter = keyFormatter;
        }

        public AzureTableSagaRepositoryContextFactory(CloudTable cloudTable,
            ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> factory,
            ISagaKeyFormatter<TSaga> keyFormatter)
            : this(new ConstCloudTableProvider<TSaga>(cloudTable), factory, keyFormatter)
        {
        }

        public void Probe(ProbeContext context)
        {
            context.Add("persistence", "azuretable");
        }

        public async Task Send<T>(ConsumeContext<T> context, IPipe<SagaRepositoryContext<TSaga, T>> next)
            where T : class
        {
            var database = _cloudTableProvider.GetCloudTable();

            var databaseContext = new AzureTableDatabaseContext<TSaga>(database, _keyFormatter);

            var repositoryContext = new AzureTableSagaRepositoryContext<TSaga, T>(databaseContext, context, _factory);

            await next.Send(repositoryContext).ConfigureAwait(false);
        }

        public async Task SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, IPipe<SagaRepositoryQueryContext<TSaga, T>> next)
            where T : class
        {
            throw new NotImplementedByDesignException("Azure Table repository does not support queries");
        }

        public async Task<T> Execute<T>(Func<SagaRepositoryContext<TSaga>, Task<T>> asyncMethod, CancellationToken cancellationToken = default)
            where T : class
        {
            var database = _cloudTableProvider.GetCloudTable();

            var databaseContext = new AzureTableDatabaseContext<TSaga>(database, _keyFormatter);
            var repositoryContext = new CosmosTableSagaRepositoryContext<TSaga>(databaseContext, cancellationToken);

            return await asyncMethod(repositoryContext).ConfigureAwait(false);
        }
    }
}
