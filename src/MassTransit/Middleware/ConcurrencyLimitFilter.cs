namespace MassTransit.Middleware
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;


    /// <summary>
    /// Limits the concurrency of the next section of the pipeline based on the concurrency limit
    /// specified.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class ConcurrencyLimitFilter<TContext> :
        Agent,
        IFilter<TContext>,
        IPipe<CommandContext<SetConcurrencyLimit>>,
        IDisposable
        where TContext : class, PipeContext
    {
        readonly int _concurrencyLimit;
        readonly SemaphoreSlim _limit;

        public ConcurrencyLimitFilter(int concurrencyLimit)
        {
            _concurrencyLimit = concurrencyLimit;

            _limit = new SemaphoreSlim(concurrencyLimit);
        }

        public void Dispose()
        {
            _limit?.Dispose();
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("concurrencyLimit");
            scope.Add("limit", _concurrencyLimit);
            scope.Add("available", _limit.CurrentCount);
        }

        [DebuggerNonUserCode]
        public async Task Send(TContext context, IPipe<TContext> next)
        {
            var waitAsyncTask = _limit.WaitAsync(context.CancellationToken);
            if (waitAsyncTask.Status != TaskStatus.RanToCompletion)
                await waitAsyncTask.ConfigureAwait(false);

            try
            {
                await next.Send(context).ConfigureAwait(false);
            }
            finally
            {
                _limit.Release();
            }
        }

        public async Task Send(CommandContext<SetConcurrencyLimit> context)
        {
            var concurrencyLimit = context.Command.ConcurrencyLimit;
            if (concurrencyLimit < 1)
                throw new ArgumentOutOfRangeException(nameof(concurrencyLimit), "The concurrency limit must be >= 1");

            var previousLimit = _concurrencyLimit;
            if (concurrencyLimit > previousLimit)
                _limit.Release(concurrencyLimit - previousLimit);
            else
            {
                for (; previousLimit > concurrencyLimit; previousLimit--)
                    await _limit.WaitAsync(context.CancellationToken).ConfigureAwait(false);
            }
        }

        protected override async Task StopAgent(StopContext context)
        {
            var slot = 0;
            try
            {
                for (; slot < _concurrencyLimit; slot++)
                    await _limit.WaitAsync(context.CancellationToken).ConfigureAwait(false);

                await base.StopAgent(context).ConfigureAwait(false);
            }
            finally
            {
                _limit.Release(slot);
            }
        }
    }
}
