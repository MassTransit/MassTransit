namespace MassTransit.MongoDbIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CollectionNameFormatters;
    using Context;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;
    using MassTransit.Saga;
    using Metadata;
    using MongoDB.Driver;
    using Pipeline;


    public class MongoDbSagaRepository<TSaga> :
        ISagaRepository<TSaga>
        where TSaga : class, IVersionedSaga
    {
        readonly IMongoCollection<TSaga> _collection;
        readonly IMongoDbSagaConsumeContextFactory _mongoDbSagaConsumeContextFactory;

        public MongoDbSagaRepository(string connectionString, string database, string collectionName = null)
            : this(new MongoClient(connectionString).GetDatabase(database), new MongoDbSagaConsumeContextFactory(), collectionName)
        {
        }

        public MongoDbSagaRepository(MongoUrl mongoUrl, string collectionName = null)
            : this(mongoUrl.Url, mongoUrl.DatabaseName, collectionName)
        {
        }

        public MongoDbSagaRepository(IMongoDatabase mongoDatabase,
            IMongoDbSagaConsumeContextFactory mongoDbSagaConsumeContextFactory,
            string collectionName = null)
            : this(mongoDatabase, mongoDbSagaConsumeContextFactory, new DefaultCollectionNameFormatter(collectionName))
        {
        }

        public MongoDbSagaRepository(IMongoDatabase database,
            IMongoDbSagaConsumeContextFactory mongoDbSagaConsumeContextFactory,
            ICollectionNameFormatter collectionNameFormatter)
        {
            _mongoDbSagaConsumeContextFactory = mongoDbSagaConsumeContextFactory;
            _collection = database.GetCollection<TSaga>(collectionNameFormatter);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("sagaRepository");

            scope.Set(new
            {
                Persistence = "mongodb",
                SagaType = TypeMetadataCache<TSaga>.ShortName,
                Properties = TypeCache<TSaga>.ReadWritePropertyCache.Select(x => x.Property.Name).ToArray()
            });
        }

        public async Task Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            if (!context.CorrelationId.HasValue)
                throw new SagaException("The CorrelationId was not specified", typeof(TSaga), typeof(T));

            if (policy.PreInsertInstance(context, out var instance))
            {
                await PreInsertSagaInstance(context, instance).ConfigureAwait(false);
            }

            if (instance == null)
            {
                instance = await _collection.Find(x => x.CorrelationId == context.CorrelationId).SingleOrDefaultAsync(context.CancellationToken)
                    .ConfigureAwait(false);
            }

            if (instance == null)
            {
                var missingSagaPipe = new MissingPipe<TSaga, T>(_collection, next, _mongoDbSagaConsumeContextFactory);

                await policy.Missing(context, missingSagaPipe).ConfigureAwait(false);
            }
            else
            {
                await SendToInstance(context, policy, next, instance).ConfigureAwait(false);
            }
        }

        public async Task SendQuery<T>(SagaQueryConsumeContext<TSaga, T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            try
            {
                List<TSaga> sagaInstances = await _collection.Find(context.Query.FilterExpression).ToListAsync().ConfigureAwait(false);

                if (!sagaInstances.Any())
                {
                    var missingPipe = new MissingPipe<TSaga, T>(_collection, next, _mongoDbSagaConsumeContextFactory);

                    await policy.Missing(context, missingPipe).ConfigureAwait(false);
                }
                else
                {
                    foreach (var instance in sagaInstances)
                    {
                        await SendToInstance(context, policy, next, instance).ConfigureAwait(false);
                    }
                }
            }
            catch (SagaException sex)
            {
                context.LogFault(sex);

                throw;
            }
            catch (Exception ex)
            {
                context.LogFault(ex);

                throw new SagaException(ex.Message, typeof(TSaga), typeof(T), Guid.Empty, ex);
            }
        }

        async Task PreInsertSagaInstance<T>(ConsumeContext<T> context, TSaga instance)
            where T : class
        {
            try
            {
                await _collection.InsertOneAsync(instance, cancellationToken: context.CancellationToken).ConfigureAwait(false);

                context.LogInsert(this, instance.CorrelationId);
            }
            catch (Exception ex)
            {
                context.LogInsertFault(this, ex, instance.CorrelationId);
            }
        }

        async Task SendToInstance<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next, TSaga instance)
            where T : class
        {
            try
            {
                SagaConsumeContext<TSaga, T> sagaConsumeContext = _mongoDbSagaConsumeContextFactory.Create(_collection, context, instance);

                sagaConsumeContext.LogUsed();

                await policy.Existing(sagaConsumeContext, next).ConfigureAwait(false);

                if (!sagaConsumeContext.IsCompleted)
                    await UpdateMongoDbSaga(context, instance).ConfigureAwait(false);
            }
            catch (SagaException sex)
            {
                context.LogFault(this, sex, instance?.CorrelationId);

                throw;
            }
            catch (Exception ex)
            {
                throw new SagaException(ex.Message, typeof(TSaga), typeof(T), instance.CorrelationId, ex);
            }
        }

        async Task UpdateMongoDbSaga(PipeContext context, TSaga instance)
        {
            instance.Version++;

            var old = await _collection.FindOneAndReplaceAsync(x => x.CorrelationId == instance.CorrelationId && x.Version < instance.Version, instance,
                cancellationToken: context.CancellationToken).ConfigureAwait(false);

            if (old == null)
                throw new MongoDbConcurrencyException("Unable to update saga. It may not have been found or may have been updated by another process.");
        }
    }
}
