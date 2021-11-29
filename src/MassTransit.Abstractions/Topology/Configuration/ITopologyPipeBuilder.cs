namespace MassTransit.Configuration
{
    /// <summary>
    /// A pipe builder used by topologies, which indicates whether the message type
    /// is either delegated (called from a sub-specification) or implemented (being called
    /// when the actual type is a subtype and this is an implemented type).
    /// </summary>
    /// <typeparam name="T">The pipe context type</typeparam>
    public interface ITopologyPipeBuilder<T> :
        IPipeBuilder<T>
        where T : class, PipeContext
    {
        /// <summary>
        /// If true, this is a delegated builder, and implemented message types
        /// and/or topology items should not be applied
        /// </summary>
        bool IsDelegated { get; }

        /// <summary>
        /// If true, this is a builder for implemented types, so don't go down
        /// the rabbit hole twice.
        /// </summary>
        bool IsImplemented { get; }

        /// <summary>
        /// Creates a new builder where the Delegated flag is true
        /// </summary>
        /// <returns></returns>
        ITopologyPipeBuilder<T> CreateDelegatedBuilder();
    }
}
