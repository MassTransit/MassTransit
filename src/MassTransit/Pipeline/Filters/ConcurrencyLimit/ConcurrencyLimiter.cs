namespace MassTransit.Pipeline.Filters.ConcurrencyLimit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;
    using Logging;


    /// <summary>
    /// A concurrency limiter (using a semaphore) which can be shared, and adjusted using a management
    /// endpoint.
    /// </summary>
    public class ConcurrencyLimiter :
        IConcurrencyLimiter
    {
        static readonly ILog _log = Logger.Get<ConcurrencyLimiter>();

        readonly int _concurrencyLimit;
        readonly string _id;
        readonly SemaphoreSlim _limit;
        DateTime _lastUpdated;

        public ConcurrencyLimiter(int concurrencyLimit, string id = null)
        {
            _concurrencyLimit = concurrencyLimit;
            _id = id;

            _limit = new SemaphoreSlim(concurrencyLimit);
            _lastUpdated = DateTime.UtcNow;
        }

        int IConcurrencyLimiter.Available => _limit.CurrentCount;
        int IConcurrencyLimiter.Limit => _concurrencyLimit;

        public Task Wait(CancellationToken cancellationToken)
        {
            return _limit.WaitAsync(cancellationToken);
        }

        public void Release()
        {
            _limit.Release();
        }

        public async Task Consume(ConsumeContext<SetConcurrencyLimit> context)
        {
            if (_id == null || _id.Equals(context.Message.Id, StringComparison.OrdinalIgnoreCase))
            {
                if (context.Message.Timestamp >= _lastUpdated)
                {
                    try
                    {
                        var concurrencyLimit = context.Message.ConcurrencyLimit;
                        if (concurrencyLimit < 1)
                            throw new ArgumentOutOfRangeException(nameof(concurrencyLimit), "The concurrency limit must be >= 1");

                        var previousLimit = _concurrencyLimit;
                        if (concurrencyLimit > previousLimit)
                            _limit.Release(concurrencyLimit - previousLimit);
                        else
                            for (; previousLimit > concurrencyLimit; previousLimit--)
                                await _limit.WaitAsync().ConfigureAwait(false);

                        _lastUpdated = context.Message.Timestamp;

                        await context.RespondAsync<ConcurrencyLimitUpdated>(new
                        {
                            Timestamp = DateTime.UtcNow,
                            context.Message.Id,
                            context.Message.ConcurrencyLimit
                        }).ConfigureAwait(false);

                        if (_log.IsDebugEnabled)
                            _log.Debug($"Set Consumer Limit: {context.Message.ConcurrencyLimit} ({context.Message.Id})");
                    }
                    catch (Exception exception)
                    {
                        if (_log.IsErrorEnabled)
                            _log.Error($"Set Consumer Limit Failed: {context.Message.ConcurrencyLimit} ({context.Message.Id})", exception);

                        throw;
                    }
                }
                else
                    throw new CommandException("The concurrency limit was updated after the command was sent.");
            }
        }
    }
}
