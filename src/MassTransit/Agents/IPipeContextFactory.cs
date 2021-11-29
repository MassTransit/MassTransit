namespace MassTransit.Agents
{
    using System.Threading;


    /// <summary>
    /// Used to create the actual context, and the active context usages
    /// </summary>
    /// <typeparam name="TContext">The context type</typeparam>
    public interface IPipeContextFactory<TContext>
        where TContext : class, PipeContext
    {
        /// <summary>
        /// Create the pipe context, which is the actual context, and not a copy of it
        /// </summary>
        /// <param name="supervisor">The supervisor containing the context</param>
        /// <returns>A handle to the pipe context</returns>
        IPipeContextAgent<TContext> CreateContext(ISupervisor supervisor);

        /// <summary>
        /// Create an active pipe context, which is a reference to the actual context
        /// </summary>
        /// <param name="supervisor">The supervisor containing the context</param>
        /// <param name="context">The actual context</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> use for the active context</param>
        /// <returns>A handle to the active context</returns>
        IActivePipeContextAgent<TContext> CreateActiveContext(ISupervisor supervisor, PipeContextHandle<TContext> context,
            CancellationToken cancellationToken = default);
    }
}
