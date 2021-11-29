namespace MassTransit
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// A handle to a PipeContext instance (of type <typeparamref name="TContext"/>), which can be disposed
    /// once it is no longer needed (or can no longer be used).
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface PipeContextHandle<TContext> :
        IAsyncDisposable
        where TContext : class, PipeContext
    {
        /// <summary>
        /// True if the context has been disposed (and can no longer be used)
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// The <typeparamref name="TContext"/> context
        /// </summary>
        Task<TContext> Context { get; }
    }
}
