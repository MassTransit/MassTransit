namespace MassTransit
{
    /// <summary>
    /// A supervisor with a set of agents (a supervisor is also an agent)
    /// </summary>
    public interface ISupervisor :
        IAgent
    {
        /// <summary>
        /// The peak number of agents active at the same time
        /// </summary>
        int PeakActiveCount { get; }

        /// <summary>
        /// The total number of agents that were added to the supervisor
        /// </summary>
        long TotalCount { get; }

        /// <summary>
        /// Add an Agent to the Supervisor
        /// </summary>
        /// <param name="agent">The agent</param>
        void Add(IAgent agent);
    }


    /// <summary>
    /// A supervisor that is also a <see cref="IPipeContextSource{TContext}"/>
    /// </summary>
    /// <typeparam name="TContext">The source context type</typeparam>
    public interface ISupervisor<out TContext> :
        ISupervisor,
        IAgent<TContext>
        where TContext : class, PipeContext
    {
    }
}
