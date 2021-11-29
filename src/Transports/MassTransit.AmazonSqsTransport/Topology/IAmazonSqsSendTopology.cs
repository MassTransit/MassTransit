namespace MassTransit
{
    using AmazonSqsTransport;


    public interface IAmazonSqsSendTopology :
        ISendTopology
    {
        new IAmazonSqsMessageSendTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class;

        SendSettings GetSendSettings(AmazonSqsEndpointAddress address);

        /// <summary>
        /// Return the error settings for the queue
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        ErrorSettings GetErrorSettings(ReceiveSettings settings);

        /// <summary>
        /// Return the dead letter settings for the queue
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        DeadLetterSettings GetDeadLetterSettings(ReceiveSettings settings);
    }
}
