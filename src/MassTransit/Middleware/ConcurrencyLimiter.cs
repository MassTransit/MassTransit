namespace MassTransit.Middleware
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;


    /// <summary>
    /// A concurrency limiter (using a semaphore) which can be shared, and adjusted using a management
    /// endpoint.
    /// </summary>
    public class ConcurrencyLimiter :
        IConcurrencyLimiter
    {
        readonly string _id;
        readonly SemaphoreSlim _limit;
        int _concurrencyLimit;
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
                        {
                            var releaseCount = concurrencyLimit - previousLimit;

                            _limit.Release(releaseCount);

                            Interlocked.Add(ref _concurrencyLimit, releaseCount);

                            _lastUpdated = context.Message.Timestamp ?? context.SentTime ?? DateTime.UtcNow;
                        }
                        else if (concurrencyLimit < previousLimit)
                        {
                            for (; previousLimit > concurrencyLimit; previousLimit--)
                            {
                                await _limit.WaitAsync().ConfigureAwait(false);

                                Interlocked.Decrement(ref _concurrencyLimit);

                                _lastUpdated = context.Message.Timestamp ?? context.SentTime ?? DateTime.UtcNow;
                            }
                        }

                        await context.RespondAsync<ConcurrencyLimitUpdated>(new
                        {
                            Timestamp = DateTime.UtcNow,
                            context.Message.Id,
                            context.Message.ConcurrencyLimit
                        }).ConfigureAwait(false);

                        LogContext.Debug?.Log("Set Consumer Limit: {ConcurrencyLimit} ({CommandId})", context.Message.ConcurrencyLimit, context.Message.Id);
                    }
                    catch (Exception exception)
                    {
                        LogContext.Error?.Log(exception, "Set Consumer Limit failed: {ConcurrencyLimit} ({CommandId})", context.Message.ConcurrencyLimit,
                            context.Message.Id);

                        throw;
                    }
                }
                else
                    throw new CommandException("The concurrency limit was updated after the command was sent.");
            }
        }
    }
}
