namespace MassTransit
{
    using Configuration;


    public interface IPublishTopologyConfigurator :
        IPublishTopology,
        ISpecification
    {
        /// <summary>
        /// Returns the specification for the message type
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <returns></returns>
        new IMessagePublishTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class;

        /// <summary>
        /// Adds a convention to the topology, which will be applied to every message type
        /// requested, to determine if a convention for the message type is available.
        /// </summary>
        /// <param name="convention">The Publish topology convention</param>
        bool TryAddConvention(IPublishTopologyConvention convention);

        /// <summary>
        /// Add a Publish topology for a specific message type
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="topology">The topology</param>
        void AddMessagePublishTopology<T>(IMessagePublishTopology<T> topology)
            where T : class;
    }
}
