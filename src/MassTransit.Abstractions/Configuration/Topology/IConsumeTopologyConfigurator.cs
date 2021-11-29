namespace MassTransit
{
    using Configuration;


    public interface IConsumeTopologyConfigurator :
        IConsumeTopology,
        ISpecification
    {
        /// <summary>
        /// Returns the specification for the message type
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <returns></returns>
        new IMessageConsumeTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class;

        /// <summary>
        /// Adds a convention to the topology, which will be applied to every message type
        /// requested, to determine if a convention for the message type is available.
        /// </summary>
        /// <param name="convention">The Consume topology convention</param>
        bool TryAddConvention(IConsumeTopologyConvention convention);

        /// <summary>
        /// Add a Consume topology for a specific message type
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="topology">The topology</param>
        void AddMessageConsumeTopology<T>(IMessageConsumeTopology<T> topology)
            where T : class;
    }
}
