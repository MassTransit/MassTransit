namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;
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


    public class BusOutboxBulkDeliveryService<TDbContext> :
        BackgroundService
        where TDbContext : DbContext
    {
        readonly IBusControl _busControl;
        readonly IsolationLevel _isolationLevel;
        readonly ILockStatementProvider _lockStatementProvider;
        readonly ILogger _logger;
        readonly IBusOutboxNotification _notification;
        readonly OutboxDeliveryServiceOptions _options;
        readonly IServiceProvider _provider;

        string _getBulkOutboxStatement;

        public BusOutboxBulkDeliveryService(IBusControl busControl, IOptions<OutboxDeliveryServiceOptions> options,
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
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _busControl.WaitForHealthStatus(BusHealthStatus.Healthy, stoppingToken).ConfigureAwait(false);

                    var count = await DeliverOutbox(_options.MessageDeliveryLimit, stoppingToken).ConfigureAwait(false);
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
                _getBulkOutboxStatement ??= _lockStatementProvider.GetBulkOutboxStatement(dbContext, resultLimit);

                async Task<int> Execute()
                {
                    using var timeoutToken = new CancellationTokenSource(_options.QueryTimeout);

                    await using var transaction = await dbContext.Database.BeginTransactionAsync(_isolationLevel, timeoutToken.Token)
                        .ConfigureAwait(false);

                    try
                    {
                        var outboxMessages = await dbContext.Set<OutboxMessage>()
                            .FromSqlRaw(_getBulkOutboxStatement)
                            .AsTracking()
                            .ToListAsync(timeoutToken.Token).ConfigureAwait(false);

                        if (outboxMessages.Count > 0)
                        {
                        #if NETSTANDARD2_0
                            await ParallelForEachAsync(outboxMessages, cancellationToken,
                                async (outboxMessage, token) => await DeliverOutboxMessage(outboxMessage, token).ConfigureAwait(false));
                        #else
                            await Parallel.ForEachAsync(outboxMessages, cancellationToken,
                                async (outboxMessage, token) => await DeliverOutboxMessage(outboxMessage, token).ConfigureAwait(false));
                        #endif

                            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
                        }

                        return outboxMessages.Count;
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

                var executeResult = executionStrategy is ExecutionStrategy
                    ? await executionStrategy.ExecuteAsync(() => Execute()).ConfigureAwait(false)
                    : await Execute().ConfigureAwait(false);

                return executeResult;
            }
            finally
            {
                if (dbContext != null)
                    await dbContext.DisposeAsync().ConfigureAwait(false);

                await scope.DisposeAsync().ConfigureAwait(false);
            }
        }

        async Task DeliverOutboxMessage(OutboxMessage message, CancellationToken cancellationToken)
        {
            message.Deserialize(SystemTextJsonMessageSerializer.Instance);

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

                LogContext.Debug?.Log("Outbox Sent: {OutboxId} {SequenceNumber} {MessageId}", message.OutboxId, message.SequenceNumber, message.MessageId);
            }
            catch (Exception ex)
            {
                LogContext.Error?.Log(ex, "Outbox Send Fault: {OutboxId} {SequenceNumber} {MessageId}", message.OutboxId, message.SequenceNumber,
                    message.MessageId);
                throw;
            }
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

        static async Task ParallelForEachAsync<T>(
            IEnumerable<T> source,
            CancellationToken cancellationToken,
            Func<T, CancellationToken, Task> body,
            int? maxConcurrency = null)
        {
            var count = maxConcurrency ?? Environment.ProcessorCount;

            var tasks = new List<Task>();
            using var semaphore = new SemaphoreSlim(count);

            foreach (var item in source)
            {
                await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        await body(item, cancellationToken).ConfigureAwait(false);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }, cancellationToken));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}
