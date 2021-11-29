namespace MassTransit.Middleware
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;


    /// <summary>
    /// Limits the number of calls through the filter to a specified count per time interval
    /// specified.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class RateLimitFilter<TContext> :
        IFilter<TContext>,
        IPipe<CommandContext<SetRateLimit>>,
        IDisposable
        where TContext : class, PipeContext
    {
        readonly TimeSpan _interval;
        readonly SemaphoreSlim _limit;
        readonly Timer _timer;
        int _count;
        int _rateLimit;

        public RateLimitFilter(int rateLimit, TimeSpan interval)
        {
            _rateLimit = rateLimit;
            _interval = interval;
            _limit = new SemaphoreSlim(rateLimit);
            _timer = new Timer(Reset, null, interval, interval);
        }

        public void Dispose()
        {
            _limit?.Dispose();
            _timer?.Dispose();
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("rateLimit");
            scope.Add("limit", _rateLimit);
            scope.Add("available", _limit.CurrentCount);
            scope.Add("interval", _interval);
        }

        [DebuggerNonUserCode]
        public Task Send(TContext context, IPipe<TContext> next)
        {
            var waitAsync = _limit.WaitAsync(context.CancellationToken);
            if (waitAsync.Status == TaskStatus.RanToCompletion)
            {
                Interlocked.Increment(ref _count);

                return next.Send(context);
            }

            async Task SendAsync()
            {
                await waitAsync.ConfigureAwait(false);

                Interlocked.Increment(ref _count);

                await next.Send(context).ConfigureAwait(false);
            }

            return SendAsync();
        }

        public async Task Send(CommandContext<SetRateLimit> context)
        {
            var rateLimit = context.Command.RateLimit;
            if (rateLimit < 1)
                throw new ArgumentOutOfRangeException(nameof(rateLimit), "The rate limit must be >= 1");

            var previousLimit = _rateLimit;
            if (rateLimit > previousLimit)
                _limit.Release(rateLimit - previousLimit);
            else
            {
                for (; previousLimit > rateLimit; previousLimit--)
                    await _limit.WaitAsync().ConfigureAwait(false);
            }

            _rateLimit = rateLimit;
        }

        void Reset(object state)
        {
            var processed = Interlocked.Exchange(ref _count, 0);
            if (processed > 0)
                _limit.Release(processed);
        }
    }
}
