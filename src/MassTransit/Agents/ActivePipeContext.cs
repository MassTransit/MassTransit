namespace MassTransit.Agents
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// An active reference to a pipe context, which is managed by an existing pipe context handle.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class ActivePipeContext<TContext> :
        ActivePipeContextHandle<TContext>
        where TContext : class, PipeContext
    {
        readonly Task<TContext> _context;
        readonly PipeContextHandle<TContext> _contextHandle;

        /// <summary>
        /// Creates the active pipe context handle, which must have completed before this instance is created. Otherwise,
        /// it would create a pretty nasty async mess that wouldn't handle faults very well (actually, it should, but I haven't tested it).
        /// </summary>
        /// <param name="contextHandle">The context handle of the actual context which is being used</param>
        /// <param name="context">The actual context, which should be a completed Task</param>
        public ActivePipeContext(PipeContextHandle<TContext> contextHandle, Task<TContext> context)
        {
            _contextHandle = contextHandle;
            _context = context;
        }

        /// <summary>
        /// Creates the active pipe context handle, which must have completed before this instance is created. Otherwise,
        /// it would create a pretty nasty async mess that wouldn't handle faults very well (actually, it should, but I haven't tested it).
        /// </summary>
        /// <param name="contextHandle">The context handle of the actual context which is being used</param>
        /// <param name="context">The actual context</param>
        public ActivePipeContext(PipeContextHandle<TContext> contextHandle, TContext context)
        {
            _contextHandle = contextHandle;
            _context = Task.FromResult(context);
        }

        bool PipeContextHandle<TContext>.IsDisposed => _contextHandle.IsDisposed;

        Task<TContext> PipeContextHandle<TContext>.Context => _context;

        async Task ActivePipeContextHandle<TContext>.Faulted(Exception exception)
        {
            // However, a fault we should dispose of the context
            await _contextHandle.DisposeAsync().ConfigureAwait(false);
        }

        public ValueTask DisposeAsync()
        {
            // An active usage doesn't actually dispose the actual context
            return default;
        }
    }
}
