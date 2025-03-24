namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using RetryPolicies;


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
        readonly IRetryPolicy _retryPolicy;

        public InboxCleanupService(IOptions<InboxCleanupServiceOptions> options, ILogger<InboxCleanupService<TDbContext>> logger,
            IServiceProvider provider)
        {
            _options = options.Value;
            _logger = logger;
            _provider = provider;

            _retryPolicy = Retry.Exponential(1000, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(3));
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

                    removed = await _retryPolicy.Retry(() => CleanUpInboxState(stoppingToken), stoppingToken).ConfigureAwait(false);
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

                using var queryTimeout = new CancellationTokenSource(_options.QueryTimeout);
                using var queryToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, queryTimeout.Token);

                var removeTimestamp = DateTime.UtcNow - _options.DuplicateDetectionWindow;

                var count = await dbContext.Set<InboxState>()
                    .Where(x => x.Delivered != null && x.Delivered.Value < removeTimestamp)
                    .OrderBy(x => x.Delivered)
                    .Take(_options.QueryMessageLimit)
                    .ExecuteDeleteAsync(queryToken.Token).ConfigureAwait(false);

                if (count > 0)
                    _logger.LogDebug("Outbox Removed {Count} expired inbox messages", count);

                return count;
            }
            finally
            {
                await scope.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}
