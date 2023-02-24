namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;


    /// <summary>
    /// The Inbox Cleanup Service is responsible for removing <see cref="InboxState" /> entries after the expiration
    /// window timeout has elapsed.
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    public class InboxCleanupService<TDbContext> :
        BackgroundService
        where TDbContext : DbContext
    {
        readonly ILogger<InboxCleanupService<TDbContext>> _logger;
        readonly InboxCleanupServiceOptions _options;
        readonly IServiceProvider _provider;

        public InboxCleanupService(IOptions<InboxCleanupServiceOptions> options, ILogger<InboxCleanupService<TDbContext>> logger,
            IServiceProvider provider)
        {
            _options = options.Value;
            _logger = logger;
            _provider = provider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var removed = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (removed == 0)
                        await Task.Delay(_options.QueryDelay, stoppingToken).ConfigureAwait(false);
                    else
                        removed = 0;

                    removed = await CleanUpInboxState(stoppingToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                }
                catch (DbUpdateConcurrencyException exception)
                {
                    _logger.LogDebug(exception,
                        "CleanUpInboxState faulted: Concurrency exceptions are expected when running multiple instances of the InboxCleanupService");
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "CleanUpInboxState faulted");
                }
            }
        }

        async Task<int> CleanUpInboxState(CancellationToken cancellationToken)
        {
            var scope = _provider.CreateAsyncScope();

            try
            {
                await using var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

                List<InboxState> inboxStates = await GetExpiredInboxStates(dbContext, cancellationToken);

                if (inboxStates.Count == 0)
                    return 0;

                dbContext.RemoveRange(inboxStates);

                using var saveTimeout = new CancellationTokenSource(_options.QueryTimeout);
                using var saveToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, saveTimeout.Token);

                var changed = await dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogDebug("Outbox Removed {Count} expired inbox messages", changed);

                return changed;
            }
            finally
            {
                await scope.DisposeAsync().ConfigureAwait(false);
            }
        }

        async Task<List<InboxState>> GetExpiredInboxStates(TDbContext dbContext, CancellationToken cancellationToken)
        {
            var messageLimit = _options.QueryMessageLimit;

            var removeTimestamp = DateTime.UtcNow - _options.DuplicateDetectionWindow;

            using var queryTimeout = new CancellationTokenSource(_options.QueryTimeout);
            using var queryToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, queryTimeout.Token);

            return await dbContext.Set<InboxState>()
                .Where(x => x.Delivered != null && x.Delivered.Value < removeTimestamp)
                .OrderBy(x => x.Delivered)
                .Take(messageLimit)
                .ToListAsync(queryToken.Token);
        }
    }
}
