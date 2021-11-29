namespace MassTransit
{
    using Configuration;


    public interface ISendTopologyConfigurator :
        ISendTopology,
        ISpecification
    {
        /// <summary>
        /// Adds a convention to the topology, which will be applied to every message type
        /// requested, to determine if a convention for the message type is available.
        /// </summary>
        /// <param name="convention">The send topology convention</param>
        bool TryAddConvention(ISendTopologyConvention convention);

        /// <summary>
        /// Add a send topology for a specific message type
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="topology">The topology</param>
        void AddMessageSendTopology<T>(IMessageSendTopology<T> topology)
            where T : class;
    }
}
