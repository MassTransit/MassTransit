namespace MassTransit
{
    using System.Threading.Tasks;


    /// <summary>
    /// A filter is a functional node in a pipeline, connected by pipes to
    /// other filters.
    /// </summary>
    /// <typeparam name="TContext">The pipe context type</typeparam>
    public interface IFilter<TContext> :
        IProbeSite
        where TContext : class, PipeContext
    {
        /// <summary>
        /// Sends a context to a filter, such that it can be processed and then passed to the
        /// specified output pipe for further processing.
        /// </summary>
        /// <param name="context">The pipe context type</param>
        /// <param name="next">The next pipe in the pipeline</param>
        /// <returns>An awaitable Task</returns>
        Task Send(TContext context, IPipe<TContext> next);
    }
}
