namespace MassTransit
{
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// An agent can be supervised, and signals when it has completed
    /// </summary>
    public interface IAgent
    {
        /// <summary>
        /// A Task which can be awaited and is completed when the agent is either ready or faulted/canceled
        /// </summary>
        Task Ready { get; }

        /// <summary>
        /// A Task which is completed when the agent has completed (should never be set to Faulted, per convention)
        /// </summary>
        Task Completed { get; }

        /// <summary>
        /// The token which indicates if the agent is in the process of stopping (or stopped)
        /// </summary>
        CancellationToken Stopping { get; }

        /// <summary>
        /// The token which indicates if the agent is stopped
        /// </summary>
        CancellationToken Stopped { get; }

        /// <summary>
        /// Stop the agent, and any supervised agents under it's control. Any faults related to stopping should
        /// be returned via this method, and not propagated to the <see cref="Completed"/> Task.
        /// </summary>
        /// <param name="context">The stop context</param>
        Task Stop(StopContext context);
    }


    /// <summary>
    /// An agent that is also a pipe context source, of the specified context type
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface IAgent<out TContext> :
        IAgent,
        IPipeContextSource<TContext>
        where TContext : class, PipeContext
    {
    }
}
