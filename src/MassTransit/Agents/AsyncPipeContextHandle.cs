namespace MassTransit.Agents
{
    using System;
    using System.Threading.Tasks;
    using Util;


    /// <summary>
    /// An asynchronously pipe context handle, which can be completed.
    /// </summary>
    /// <typeparam name="TContext">The context type</typeparam>
    public class AsyncPipeContextHandle<TContext> :
        IAsyncPipeContextHandle<TContext>
        where TContext : class, PipeContext
    {
        readonly TaskCompletionSource<TContext> _context;
        readonly TaskCompletionSource<DateTime> _inactive;

        /// <summary>
        /// Creates the handle
        /// </summary>
        public AsyncPipeContextHandle()
        {
            _context = TaskUtil.GetTask<TContext>();
            _inactive = TaskUtil.GetTask<DateTime>();
        }

        bool PipeContextHandle<TContext>.IsDisposed => _inactive.Task.IsCompleted;

        Task<TContext> PipeContextHandle<TContext>.Context => _context.Task;

        Task IAsyncPipeContextHandle<TContext>.Created(TContext context)
        {
            _context.SetResult(context);

            return Task.CompletedTask;
        }

        Task IAsyncPipeContextHandle<TContext>.CreateCanceled()
        {
            _context.SetCanceled();

            return Task.CompletedTask;
        }

        Task IAsyncPipeContextHandle<TContext>.CreateFaulted(Exception exception)
        {
            _context.SetException(exception);

            return Task.CompletedTask;
        }

        Task IAsyncPipeContextHandle<TContext>.Faulted(Exception exception)
        {
            _inactive.TrySetException(exception);

            return Task.CompletedTask;
        }

        ValueTask IAsyncDisposable.DisposeAsync()
        {
            _inactive.TrySetResult(DateTime.UtcNow);

            return default;
        }
    }
}
