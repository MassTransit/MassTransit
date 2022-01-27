namespace MassTransit.DynamoDb.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.DynamoDBv2;
    using Amazon.DynamoDBv2.DataModel;
    using DynamoDb;
    using GreenPipes;
    using Saga;


    public class DynamoDbSagaRepositoryContextFactory<TSaga> :
        ISagaRepositoryContextFactory<TSaga>
        where TSaga : class, ISagaVersion
    {
        readonly Func<IDynamoDBContext> _databaseFactory;
        readonly ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> _factory;
        readonly DynamoDbSagaRepositoryOptions<TSaga> _options;

        public DynamoDbSagaRepositoryContextFactory(IAmazonDynamoDB dynamoDbClient,DynamoDBContextConfig config , ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> factory,
            DynamoDbSagaRepositoryOptions<TSaga> options)
        {
            IDynamoDBContext DatabaseFactory()
            {
                return new DynamoDBContext(dynamoDbClient, config);
            }

            _databaseFactory = DatabaseFactory;

            _factory = factory;
            _options = options;
        }

        public DynamoDbSagaRepositoryContextFactory(Func<IDynamoDBContext> databaseFactory, ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> factory,
            DynamoDbSagaRepositoryOptions<TSaga> options)
        {
            _databaseFactory = databaseFactory;

            _factory = factory;
            _options = options;
        }

        public void Probe(ProbeContext context)
        {
            context.Add("persistence", "dynamodb");
        }

        public async Task Send<T>(ConsumeContext<T> context, IPipe<SagaRepositoryContext<TSaga, T>> next)
            where T : class
        {
            var database = _databaseFactory();

            var databaseContext = new DynamoDbDatabaseContext<TSaga>(database, _options);
            try
            {
         //       if (_options.ConcurrencyMode == ConcurrencyMode.Pessimistic)
           //         await databaseContext.Lock(context.CorrelationId.Value, context.CancellationToken).ConfigureAwait(false);

                var repositoryContext = new DynamoDbSagaRepositoryContext<TSaga, T>(databaseContext, context, _factory);

                await next.Send(repositoryContext).ConfigureAwait(false);
            }
            finally
            {
                await databaseContext.DisposeAsync().ConfigureAwait(false);
            }
        }

        public async Task SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, IPipe<SagaRepositoryQueryContext<TSaga, T>> next)
            where T : class
        {
            throw new NotImplementedByDesignException("DynamoDb saga repository does not support queries");
        }

        public async Task<T> Execute<T>(Func<SagaRepositoryContext<TSaga>, Task<T>> asyncMethod, CancellationToken cancellationToken = default)
            where T : class
        {
            var database = _databaseFactory();

            var databaseContext = new DynamoDbDatabaseContext<TSaga>(database, _options);
            try
            {
                var repositoryContext = new DynamoDbSagaRepositoryContext<TSaga>(databaseContext, cancellationToken);

                return await asyncMethod(repositoryContext).ConfigureAwait(false);
            }
            finally
            {
                await databaseContext.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}
