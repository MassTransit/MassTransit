namespace MassTransit.Agents
{
    using System;
    using System.Threading.Tasks;


    public class ConstantPipeContextHandle<TContext> :
        PipeContextHandle<TContext>
        where TContext : class, PipeContext
    {
        readonly TContext _context;
        bool _disposed;

        public ConstantPipeContextHandle(TContext context)
        {
            _context = context;

            Context = Task.FromResult(context);
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            if (_disposed)
                return;

            switch (_context)
            {
                case IAsyncDisposable asyncDisposable:
                    await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                    break;
                case IDisposable disposable:
                    disposable.Dispose();
                    break;
            }

            _disposed = true;
        }

        bool PipeContextHandle<TContext>.IsDisposed => _disposed;

        public Task<TContext> Context { get; }
    }
}
