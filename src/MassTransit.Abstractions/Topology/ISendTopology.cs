namespace MassTransit
{
    using Configuration;


    public interface ISendTopology :
        ISendTopologyConfigurationObserverConnector
    {
        IDeadLetterQueueNameFormatter DeadLetterQueueNameFormatter { get; }

        IErrorQueueNameFormatter ErrorQueueNameFormatter { get; }

        /// <summary>
        /// Returns the specification for the message type
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <returns></returns>
        IMessageSendTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class;
    }
}
