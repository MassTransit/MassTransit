namespace MassTransit.MongoDbIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using MongoDB.Driver;
    using Outbox;


    /// <summary>
    /// The Inbox Cleanup Service is responsible for removing <see cref="InboxState" /> entries after the expiration
    /// window timeout has elapsed.
    /// </summary>
    public class InboxCleanupService :
        BackgroundService
    {
        readonly ILogger<InboxCleanupService> _logger;
        readonly InboxCleanupServiceOptions _options;
        readonly IServiceProvider _provider;

        public InboxCleanupService(IOptions<InboxCleanupServiceOptions> options, ILogger<InboxCleanupService> logger, IServiceProvider provider)
        {
            _options = options.Value;
            _logger = logger;
            _provider = provider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            long removed = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (removed == 0)
                        await Task.Delay(_options.QueryDelay, stoppingToken).ConfigureAwait(false);

                    removed = await CleanUpInboxState(stoppingToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException exception) when (exception.CancellationToken == stoppingToken)
                {
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "CleanUpInboxState faulted");
                    removed = 0;
                }
            }
        }

        async Task<long> CleanUpInboxState(CancellationToken cancellationToken)
        {
            var scope = _provider.CreateScope();

            try
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
                try
                {
                    MongoDbCollectionContext<InboxState> collection = dbContext.GetCollection<InboxState>();

                    var result = await RemoveInboxStates(collection, cancellationToken);
                    if (result.DeletedCount == 0)
                        return 0;

                    _logger.LogDebug("Outbox Removed {Count} expired inbox messages", result.DeletedCount);

                    return result.DeletedCount;
                }
                finally
                {
                    if (dbContext is IDisposable disposable)
                        disposable.Dispose();
                }
            }
            finally
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                if (scope is IAsyncDisposable disposable)
                    await disposable.DisposeAsync();
                else
                    scope.Dispose();
            }
        }

        Task<DeleteResult> RemoveInboxStates(MongoDbCollectionContext<InboxState> dbContext, CancellationToken cancellationToken)
        {
            var removeTimestamp = DateTime.UtcNow - _options.DuplicateDetectionWindow;

            using var queryTimeout = new CancellationTokenSource(_options.QueryTimeout);
            using var queryToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, queryTimeout.Token);

            FilterDefinitionBuilder<InboxState> builder = Builders<InboxState>.Filter;
            FilterDefinition<InboxState> filter = builder.Not(builder.Eq(x => x.Delivered, null)) & builder.Lt(x => x.Delivered, removeTimestamp);

            return dbContext.DeleteMany(filter, cancellationToken);
        }
    }
}
