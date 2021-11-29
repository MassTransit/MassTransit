namespace MassTransit
{
    using Configuration;


    public interface IConsumeTopology :
        IConsumeTopologyConfigurationObserverConnector
    {
        /// <summary>
        /// Returns the specification for the message type
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <returns></returns>
        IMessageConsumeTopology<T> GetMessageTopology<T>()
            where T : class;

        /// <summary>
        /// Create a temporary endpoint name, using the specified tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        string CreateTemporaryQueueName(string tag);
    }
}
