namespace MassTransit.Agents
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// An active, in-use reference to a pipe context.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface ActivePipeContextHandle<TContext> :
        PipeContextHandle<TContext>
        where TContext : class, PipeContext
    {
        /// <summary>
        /// If the use of this context results in a fault which should cause the context to be disposed, this method signals that behavior to occur.
        /// </summary>
        /// <param name="exception">The bad thing that happened</param>
        Task Faulted(Exception exception);
    }
}
