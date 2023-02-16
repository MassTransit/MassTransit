namespace MassTransit.Agents
{
    using System;
    using System.Threading.Tasks;
    using Middleware;
    using Util;


    /// <summary>
    /// A PipeContext, which as an agent can be Stopped, which disposes of the context making it unavailable
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class PipeContextAgent<TContext> :
        Agent,
        IPipeContextAgent<TContext>
        where TContext : class, PipeContext
    {
        readonly Task<TContext> _context;
        readonly TaskCompletionSource<DateTime> _inactive;

        public PipeContextAgent(TContext context)
            : this(Task.FromResult(context))
        {
        }

        public PipeContextAgent(Task<TContext> context)
        {
            _context = context;
            _inactive = TaskUtil.GetTask<DateTime>();

            SetReady(_context);
        }

        bool PipeContextHandle<TContext>.IsDisposed => _inactive.Task.IsCompleted;

        Task<TContext> PipeContextHandle<TContext>.Context => _context;

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            // dispose only once
            if (!_inactive.TrySetResult(DateTime.UtcNow))
                return;

            if (_context.Status == TaskStatus.RanToCompletion)
            {
                switch (_context.Result)
                {
                    case IAsyncDisposable asyncDisposable:
                        await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                        break;
                    case IDisposable disposable:
                        disposable.Dispose();
                        break;
                }
            }

            SetCompleted(_inactive.Task);
        }

        /// <inheritdoc />
        protected override async Task StopAgent(StopContext context)
        {
            await DisposeAsync().ConfigureAwait(false);
        }
    }
}
