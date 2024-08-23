namespace MassTransit.MongoDbIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Middleware;
    using Middleware.Outbox;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using Outbox;
    using Serialization;


    public class BusOutboxDeliveryService :
        BackgroundService
    {
        readonly IBusControl _busControl;
        readonly ILogger _logger;
        readonly IBusOutboxNotification _notification;
        readonly OutboxDeliveryServiceOptions _options;
        readonly IServiceProvider _provider;

        public BusOutboxDeliveryService(IBusControl busControl, IOptions<OutboxDeliveryServiceOptions> options,
            IBusOutboxNotification notification, ILogger<BusOutboxDeliveryService> logger, IServiceProvider provider)
        {
            _busControl = busControl;
            _notification = notification;
            _provider = provider;
            _logger = logger;

            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _busControl.WaitForHealthStatus(BusHealthStatus.Healthy, stoppingToken).ConfigureAwait(false);

                    var count = await ProcessOutboxes(stoppingToken).ConfigureAwait(false);
                    if (count > 0)
                        continue;

                    await _notification.WaitForDelivery(stoppingToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "ProcessOutboxes faulted");
                }
            }
        }

        async Task<int> ProcessOutboxes(CancellationToken cancellationToken)
        {
            List<Guid> outboxIds = (await GetOutboxes(_options.QueryMessageLimit, cancellationToken).ConfigureAwait(false)).ToList();

            await Task.WhenAll(outboxIds.Select(outboxId => DeliverOutbox(outboxId, cancellationToken))).ConfigureAwait(false);

            return outboxIds.Count;
        }

        async Task<IEnumerable<Guid>> GetOutboxes(int resultLimit, CancellationToken cancellationToken)
        {
            var scope = _provider.CreateAsyncScope();

            try
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();

                MongoDbCollectionContext<OutboxState> collection = dbContext.GetCollection<OutboxState>();

                FilterDefinitionBuilder<OutboxState> builder = Builders<OutboxState>.Filter;

                List<Guid> outboxIds = await collection
                    .Find(builder.Empty)
                    .Sort(Builders<OutboxState>.Sort.Ascending(x => x.Created))
                    .Limit(resultLimit)
                    .Project(x => x.OutboxId)
                    .ToListAsync(cancellationToken).ConfigureAwait(false);

                return outboxIds;
            }
            finally
            {
                await scope.DisposeAsync().ConfigureAwait(false);
            }
        }

        async Task DeliverOutbox(Guid outboxId, CancellationToken cancellationToken)
        {
            var scope = _provider.CreateAsyncScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();

            MongoDbCollectionContext<OutboxState> stateCollection = dbContext.GetCollection<OutboxState>();
            MongoDbCollectionContext<OutboxMessage> messageCollection = dbContext.GetCollection<OutboxMessage>();

            FilterDefinitionBuilder<OutboxState> builder = Builders<OutboxState>.Filter;
            FilterDefinition<OutboxState> filter = builder.Eq(x => x.OutboxId, outboxId);

            try
            {
                async Task<bool> Execute()
                {
                    using var timeoutToken = new CancellationTokenSource(_options.QueryTimeout);

                    await dbContext.BeginTransaction(cancellationToken).ConfigureAwait(false);

                    try
                    {
                        UpdateDefinition<OutboxState> update = Builders<OutboxState>.Update.Set(x => x.LockToken, ObjectId.GenerateNewId());

                        var outboxState = await stateCollection.Lock(filter, update, cancellationToken).ConfigureAwait(false);

                        bool continueProcessing;

                        if (outboxState == null)
                        {
                            outboxState = new OutboxState
                            {
                                OutboxId = outboxId,
                                Version = 1
                            };

                            await stateCollection.InsertOne(outboxState, cancellationToken).ConfigureAwait(false);

                            continueProcessing = true;
                        }
                        else
                        {
                            if (outboxState.Delivered.HasValue)
                            {
                                await RemoveOutbox(messageCollection, stateCollection, outboxState, cancellationToken).ConfigureAwait(false);

                                continueProcessing = false;
                            }
                            else
                            {
                                continueProcessing = await DeliverOutboxMessages(messageCollection, outboxState, cancellationToken).ConfigureAwait(false);

                                outboxState.Version++;

                                FilterDefinition<OutboxState> updateFilter =
                                    builder.Eq(x => x.OutboxId, outboxId) & builder.Lt(x => x.Version, outboxState.Version);

                                await stateCollection.FindOneAndReplace(updateFilter, outboxState, cancellationToken).ConfigureAwait(false);
                            }
                        }

                        await dbContext.CommitTransaction(cancellationToken).ConfigureAwait(false);

                        return continueProcessing;
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (MongoCommandException)
                    {
                        await AbortTransaction(dbContext).ConfigureAwait(false);
                        throw;
                    }
                    catch (Exception)
                    {
                        await AbortTransaction(dbContext).ConfigureAwait(false);
                        throw;
                    }
                }

                var continueProcessing = true;
                while (continueProcessing)
                    continueProcessing = await Execute().ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                LogContext.Warning?.Log(ex, "Outbox Delivery Fault: {OutboxId}", outboxId);
            }
            finally
            {
                await scope.DisposeAsync().ConfigureAwait(false);
            }
        }

        static async Task RemoveOutbox(MongoDbCollectionContext<OutboxMessage> messageCollection, MongoDbCollectionContext<OutboxState> stateCollection,
            OutboxState outboxState, CancellationToken cancellationToken)
        {
            var messages = await messageCollection.DeleteMany(Builders<OutboxMessage>.Filter.Eq(x => x.OutboxId, outboxState.OutboxId), cancellationToken)
                .ConfigureAwait(false);

            if (messages.DeletedCount > 0)
                LogContext.Debug?.Log("Outbox removed {Count} messages: {MessageId}", messages.DeletedCount, outboxState.OutboxId);

            await stateCollection.DeleteOne(Builders<OutboxState>.Filter.Eq(x => x.OutboxId, outboxState.OutboxId), cancellationToken)
                .ConfigureAwait(false);
        }

        async Task<bool> DeliverOutboxMessages(MongoDbCollectionContext<OutboxMessage> messageCollection, OutboxState outboxState,
            CancellationToken
                cancellationToken)
        {
            var messageLimit = _options.MessageDeliveryLimit;

            var lastSequenceNumber = outboxState.LastSequenceNumber ?? 0;

            FilterDefinitionBuilder<OutboxMessage> builder = Builders<OutboxMessage>.Filter;
            FilterDefinition<OutboxMessage> filter = builder.Eq(x => x.OutboxId, outboxState.OutboxId) & builder.Gt(x => x.SequenceNumber, lastSequenceNumber);

            List<OutboxMessage> messages = await messageCollection.Find(filter)
                .Sort(Builders<OutboxMessage>.Sort.Ascending(x => x.SequenceNumber))
                .Limit(messageLimit + 1)
                .ToListAsync(cancellationToken);

            var sentSequenceNumber = 0L;

            var messageCount = 0;
            var messageIndex = 0;
            for (; messageIndex < messages.Count && messageCount < messageLimit; messageIndex++)
            {
                var message = messages[messageIndex];

                message.Deserialize(SystemTextJsonMessageSerializer.Instance);

                if (message.DestinationAddress == null)
                {
                    LogContext.Warning?.Log("Outbox message DestinationAddress not present: {SequenceNumber} {MessageId}", message.SequenceNumber,
                        message.MessageId);
                }
                else
                {
                    try
                    {
                        using var sendToken = new CancellationTokenSource(_options.MessageDeliveryTimeout);
                        using var token = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, sendToken.Token);

                        var pipe = new OutboxMessageSendPipe(message, message.DestinationAddress);

                        var endpoint = await _busControl.GetSendEndpoint(message.DestinationAddress).ConfigureAwait(false);

                        StartedActivity? activity = LogContext.Current?.StartOutboxDeliverActivity(message);
                        StartedInstrument? instrument = LogContext.Current?.StartOutboxDeliveryInstrument(message);
                        try
                        {
                            await endpoint.Send(new SerializedMessageBody(), pipe, token.Token).ConfigureAwait(false);
                        }
                        catch (Exception exception)
                        {
                            activity?.AddExceptionEvent(exception);
                            instrument?.AddException(exception);

                            throw;
                        }
                        finally
                        {
                            activity?.Stop();
                            instrument?.Stop();
                        }

                        sentSequenceNumber = message.SequenceNumber;

                        LogContext.Debug?.Log("Outbox Sent: {OutboxId} {SequenceNumber} {MessageId}", message.OutboxId, sentSequenceNumber, message.MessageId);
                    }
                    catch (Exception ex)
                    {
                        LogContext.Warning?.Log(ex, "Outbox Send Fault: {OutboxId} {SequenceNumber} {MessageId}", message.OutboxId, message.SequenceNumber,
                            message.MessageId);

                        break;
                    }

                    await messageCollection.DeleteOne(Builders<OutboxMessage>.Filter.Eq(x => x.MessageId, message.MessageId), cancellationToken)
                        .ConfigureAwait(false);

                    messageCount++;
                }
            }

            if (sentSequenceNumber > 0)
                outboxState.LastSequenceNumber = sentSequenceNumber;

            if (messageIndex == messages.Count && messages.Count < messageLimit)
            {
                outboxState.Delivered = DateTime.UtcNow;

                LogContext.Debug?.Log("Outbox Delivered: {OutboxId} {Delivered}", outboxState.OutboxId, outboxState.Delivered);
            }

            return true;
        }

        static async Task AbortTransaction(MongoDbContext dbContext)
        {
            try
            {
                await dbContext.AbortTransaction(CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception innerException)
            {
                LogContext.Warning?.Log(innerException, "Transaction rollback failed");
            }
        }
    }
}
