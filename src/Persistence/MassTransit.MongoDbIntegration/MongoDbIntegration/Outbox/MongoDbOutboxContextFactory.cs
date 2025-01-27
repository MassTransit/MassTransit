namespace MassTransit.MongoDbIntegration.Outbox
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Middleware;
    using MongoDB.Bson;
    using MongoDB.Driver;


    public class MongoDbOutboxContextFactory :
        IOutboxContextFactory<MongoDbContext>
    {
        readonly MongoDbContext _dbContext;
        readonly IServiceProvider _provider;

        public MongoDbOutboxContextFactory(MongoDbContext dbContext, IServiceProvider provider)
        {
            _dbContext = dbContext;
            _provider = provider;
        }

        public async Task Send<T>(ConsumeContext<T> context, OutboxConsumeOptions options, IPipe<OutboxConsumeContext<T>> next)
            where T : class
        {
            var updateReceiveCount = true;

            var messageId = context.GetOriginalMessageId() ?? throw new MessageException(typeof(T), "MessageId required to use the outbox");

            FilterDefinitionBuilder<InboxState> builder = Builders<InboxState>.Filter;
            FilterDefinition<InboxState> filter = builder.Eq(x => x.MessageId, messageId) & builder.Eq(x => x.ConsumerId, options.ConsumerId);

            async Task<bool> Execute()
            {
                await _dbContext.BeginTransaction(context.CancellationToken).ConfigureAwait(false);

                try
                {
                    MongoDbCollectionContext<InboxState> inboxStateCollection = _dbContext.GetCollection<InboxState>();

                    UpdateDefinition<InboxState> update = Builders<InboxState>.Update.Set(x => x.LockToken, ObjectId.GenerateNewId());

                    var inboxState = await inboxStateCollection.Lock(filter, update, context.CancellationToken).ConfigureAwait(false);

                    bool continueProcessing;

                    if (inboxState == null)
                    {
                        inboxState = new InboxState
                        {
                            MessageId = messageId,
                            ConsumerId = options.ConsumerId,
                            Received = DateTime.UtcNow,
                            ReceiveCount = 0
                        };

                        await inboxStateCollection.InsertOne(inboxState, context.CancellationToken).ConfigureAwait(false);

                        continueProcessing = true;
                    }
                    else
                    {
                        if (updateReceiveCount)
                        {
                            inboxState.ReceiveCount++;
                            await inboxStateCollection.FindOneAndReplace(filter, inboxState, context.CancellationToken).ConfigureAwait(false);
                        }

                        updateReceiveCount = false;

                        var outboxContext = new MongoDbOutboxConsumeContext<T>(context, options, _provider, _dbContext, inboxState);

                        await next.Send(outboxContext).ConfigureAwait(false);

                        inboxState.Version++;

                        FilterDefinition<InboxState> updateFilter = builder.Eq(x => x.MessageId, messageId) & builder.Eq(x => x.ConsumerId, options.ConsumerId)
                            & builder.Lt(x => x.Version, inboxState.Version);

                        await inboxStateCollection.FindOneAndReplace(updateFilter, inboxState, context.CancellationToken).ConfigureAwait(false);

                        continueProcessing = outboxContext.ContinueProcessing;
                    }

                    await _dbContext.CommitTransaction(context.CancellationToken).ConfigureAwait(false);

                    return continueProcessing;
                }
                catch (MongoCommandException)
                {
                    await AbortTransaction().ConfigureAwait(false);
                    throw;
                }
                catch (Exception)
                {
                    await AbortTransaction().ConfigureAwait(false);
                    throw;
                }
            }

            var continueProcessing = true;
            while (continueProcessing)
                continueProcessing = await Execute().ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("outboxContextFactory");
            scope.Add("provider", "mongoDb");
        }

        async Task AbortTransaction()
        {
            try
            {
                await _dbContext.AbortTransaction(CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception innerException)
            {
                LogContext.Warning?.Log(innerException, "Transaction rollback failed");
            }
        }
    }
}
