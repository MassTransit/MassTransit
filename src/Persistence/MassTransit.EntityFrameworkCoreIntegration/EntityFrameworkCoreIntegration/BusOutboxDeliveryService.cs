namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Internals;
    using Logging;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Middleware;
    using Middleware.Outbox;
    using Serialization;
    using Util;


    public class BusOutboxDeliveryService<TDbContext> :
        BackgroundService
        where TDbContext : DbContext
    {
        readonly IBusControl _busControl;
        readonly IsolationLevel _isolationLevel;
        readonly ILockStatementProvider _lockStatementProvider;
        readonly ILogger _logger;
        readonly IBusOutboxNotification _notification;
        readonly OutboxDeliveryServiceOptions _options;
        readonly Func<TDbContext, Guid, long, int, IAsyncEnumerable<OutboxMessage>> _outboxMessagesQuery;
        readonly IServiceProvider _provider;

        string _getOutboxIdStatement;

        public BusOutboxDeliveryService(IBusControl busControl, IOptions<OutboxDeliveryServiceOptions> options,
            IOptions<EntityFrameworkOutboxOptions> outboxOptions, IBusOutboxNotification notification,
            ILogger<BusOutboxDeliveryService<TDbContext>> logger, IServiceProvider provider)
        {
            _busControl = busControl;
            _notification = notification;
            _provider = provider;
            _logger = logger;

            _options = options.Value;

            _lockStatementProvider = outboxOptions.Value.LockStatementProvider;
            _isolationLevel = outboxOptions.Value.IsolationLevel;

            _outboxMessagesQuery = EF.CompileAsyncQuery((TDbContext context, Guid outboxId, long lastSequenceNumber, int limit) =>
                context.Set<OutboxMessage>()
                    .Where(x => x.OutboxId == outboxId && x.SequenceNumber > lastSequenceNumber)
                    .OrderBy(x => x.SequenceNumber)
                    .Take(limit)
                    .AsNoTracking());
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var algorithm = new RequestRateAlgorithm(new RequestRateAlgorithmOptions
            {
                PrefetchCount = _options.QueryMessageLimit,
                RequestResultLimit = 10,
            });

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _busControl.WaitForHealthStatus(BusHealthStatus.Healthy, stoppingToken).ConfigureAwait(false);

                    var count = await algorithm.Run(DeliverOutbox, stoppingToken).ConfigureAwait(false);
                    if (count > 0)
                        continue;

                    await _notification.WaitForDelivery(stoppingToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                }
                catch (DbUpdateConcurrencyException)
                {
                }
                catch (InvalidOperationException exception) when (exception.InnerException != null
                                                                  && exception.InnerException.Message.Contains("concurrent update"))
                {
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "ProcessMessageBatch faulted");
                }
            }
        }

        async Task<int> DeliverOutbox(int resultLimit, CancellationToken cancellationToken)
        {
            var scope = _provider.CreateAsyncScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

            try
            {
                _getOutboxIdStatement ??= _lockStatementProvider.GetOutboxStatement(dbContext);

                async Task<int> Execute()
                {
                    var lockId = NewId.NextGuid();

                    using var timeoutToken = new CancellationTokenSource(_options.QueryTimeout);

                    await using var transaction = await dbContext.Database.BeginTransactionAsync(_isolationLevel, timeoutToken.Token)
                        .ConfigureAwait(false);

                    try
                    {
                        List<OutboxState> outboxStateList = await dbContext.Set<OutboxState>()
                            .FromSqlRaw(_getOutboxIdStatement)
                            .AsTracking()
                            .ToListAsync(timeoutToken.Token).ConfigureAwait(false);
                        var outboxState = outboxStateList.SingleOrDefault();

                        if (outboxState == null)
                            return -1;

                        outboxState.LockId = lockId;

                        dbContext.Update(outboxState);
                        await dbContext.SaveChangesAsync(timeoutToken.Token).ConfigureAwait(false);

                        int continueProcessing;

                        if (outboxState.Delivered.HasValue)
                        {
                            await RemoveOutbox(dbContext, outboxState, cancellationToken).ConfigureAwait(false);

                            continueProcessing = 0;
                        }
                        else
                            continueProcessing = await DeliverOutboxMessages(dbContext, outboxState, cancellationToken).ConfigureAwait(false);

                        await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);

                        return continueProcessing;
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (InvalidOperationException exception) when (exception.InnerException != null
                                                                      && exception.InnerException.Message.Contains("concurrent update"))
                    {
                        throw;
                    }
                    catch (Exception)
                    {
                        await RollbackTransaction(transaction).ConfigureAwait(false);
                        throw;
                    }
                }

                var executionStrategy = dbContext.Database.CreateExecutionStrategy();

                var messageCount = 0;
                while (messageCount < resultLimit)
                {
                    var executeResult = executionStrategy is ExecutionStrategy
                        ? await executionStrategy.ExecuteAsync(() => Execute()).ConfigureAwait(false)
                        : await Execute().ConfigureAwait(false);

                    if (executeResult < 0)
                        break;

                    if (executeResult > 0)
                        messageCount += executeResult;
                }

                return messageCount;
            }
            finally
            {
                if (dbContext != null)
                    await dbContext.DisposeAsync().ConfigureAwait(false);

                await scope.DisposeAsync().ConfigureAwait(false);
            }
        }

        static async Task RemoveOutbox(TDbContext dbContext, OutboxState outboxState, CancellationToken cancellationToken)
        {
            List<OutboxMessage> messages = await dbContext.Set<OutboxMessage>()
                .Where(x => x.OutboxId == outboxState.OutboxId)
                .ToListAsync(cancellationToken);

            dbContext.RemoveRange(messages);

            dbContext.Remove(outboxState);

            await dbContext.SaveChangesAsync(cancellationToken);

            if (messages.Count > 0)
                LogContext.Debug?.Log("Outbox removed {Count} messages: {OutboxId}", messages.Count, outboxState);
        }

        async Task<int> DeliverOutboxMessages(TDbContext dbContext, OutboxState outboxState, CancellationToken cancellationToken)
        {
            var messageLimit = _options.MessageDeliveryLimit;

            var hasLastSequenceNumber = outboxState.LastSequenceNumber.HasValue;

            var lastSequenceNumber = outboxState.LastSequenceNumber ?? 0;

            IList<OutboxMessage> messages = await _outboxMessagesQuery(dbContext, outboxState.OutboxId, lastSequenceNumber, messageLimit)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            var sentSequenceNumber = 0L;

            var saveChanges = false;

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
                        catch (Exception ex)
                        {
                            activity?.AddExceptionEvent(ex);
                            instrument?.AddException(ex);
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

                    dbContext.Remove((object)message);

                    saveChanges = true;

                    messageCount++;
                }
            }

            if (sentSequenceNumber > 0)
            {
                outboxState.LastSequenceNumber = sentSequenceNumber;
                dbContext.Update(outboxState);

                saveChanges = true;
            }

            if (messageIndex == messages.Count && messages.Count < messageLimit)
            {
                outboxState.Delivered = DateTime.UtcNow;

                if (hasLastSequenceNumber == false)
                {
                    dbContext.Remove(outboxState);
                    dbContext.RemoveRange(messages);
                }
                else
                    dbContext.Update(outboxState);

                saveChanges = true;

                LogContext.Debug?.Log("Outbox Delivered: {OutboxId} {Delivered}", outboxState.OutboxId, outboxState.Delivered);
            }

            if (saveChanges)
                await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return messageCount;
        }

        static async Task RollbackTransaction(IDbContextTransaction transaction)
        {
            try
            {
                await transaction.RollbackAsync(CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception innerException)
            {
                LogContext.Warning?.Log(innerException, "Transaction rollback failed");
            }
        }
    }
}
