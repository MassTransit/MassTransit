namespace MassTransit.Agents
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Util;


    /// <summary>
    /// A PipeContext, which as an agent can be Stopped, which disposes of the context making it unavailable
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class AsyncPipeContextAgent<TContext> :
        IAsyncPipeContextAgent<TContext>
        where TContext : class, PipeContext
    {
        readonly IPipeContextAgent<TContext> _agent;
        readonly TaskCompletionSource<TContext> _context;

        public AsyncPipeContextAgent()
        {
            _context = TaskUtil.GetTask<TContext>();

            _agent = new PipeContextAgent<TContext>(_context.Task);
        }

        bool PipeContextHandle<TContext>.IsDisposed => _agent.IsDisposed;

        Task<TContext> PipeContextHandle<TContext>.Context => _agent.Context;

        ValueTask IAsyncDisposable.DisposeAsync()
        {
            return _agent.DisposeAsync();
        }

        Task IAgent.Ready => _agent.Ready;
        Task IAgent.Completed => _agent.Completed;

        CancellationToken IAgent.Stopping => _agent.Stopping;
        CancellationToken IAgent.Stopped => _agent.Stopped;

        Task IAgent.Stop(StopContext context)
        {
            return _agent.Stop(context);
        }

        Task IAsyncPipeContextHandle<TContext>.Created(TContext context)
        {
            _context.SetResult(context);

            return Task.CompletedTask;
        }

        Task IAsyncPipeContextHandle<TContext>.CreateCanceled()
        {
            _context.SetCanceled();

            return _agent.Stop("Create Canceled", CancellationToken.None);
        }

        Task IAsyncPipeContextHandle<TContext>.CreateFaulted(Exception exception)
        {
            _context.SetException(exception);

            return _agent.Stop($"Create Faulted: {exception.GetBaseException().Message}", CancellationToken.None);
        }

        Task IAsyncPipeContextHandle<TContext>.Faulted(Exception exception)
        {
            return _agent.Stop($"Faulted: {exception.GetBaseException().Message}", CancellationToken.None);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return _agent.ToString();
        }
    }
}
