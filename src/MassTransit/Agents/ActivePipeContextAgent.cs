namespace MassTransit.Agents
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Middleware;


    /// <summary>
    /// An Agent Provocateur that uses a context handle for the activate state of the agent
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class ActivePipeContextAgent<TContext> :
        Agent,
        IActivePipeContextAgent<TContext>
        where TContext : class, PipeContext
    {
        static readonly string _caption = $"Active<{typeof(TContext).Name}>";

        readonly ActivePipeContextHandle<TContext> _contextHandle;

        public ActivePipeContextAgent(ActivePipeContextHandle<TContext> context)
        {
            _contextHandle = context;

            context.Context.ContinueWith(SetReady, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);
            context.Context.ContinueWith(SetFaulted, CancellationToken.None, TaskContinuationOptions.NotOnRanToCompletion, TaskScheduler.Default);
        }

        bool PipeContextHandle<TContext>.IsDisposed => _contextHandle.IsDisposed;

        Task<TContext> PipeContextHandle<TContext>.Context => _contextHandle.Context;

        Task ActivePipeContextHandle<TContext>.Faulted(Exception exception)
        {
            return _contextHandle.Faulted(exception);
        }

        ValueTask IAsyncDisposable.DisposeAsync()
        {
            return _contextHandle.DisposeAsync();
        }

        /// <inheritdoc />
        protected override async Task StopAgent(StopContext context)
        {
            if (_contextHandle.Context.Status == TaskStatus.RanToCompletion)
                await _contextHandle.DisposeAsync().ConfigureAwait(false);

            SetCompleted(Task.CompletedTask);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return _caption;
        }
    }
}
