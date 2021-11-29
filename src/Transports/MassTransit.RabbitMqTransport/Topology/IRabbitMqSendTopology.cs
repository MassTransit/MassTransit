namespace MassTransit
{
    using RabbitMqTransport;


    public interface IRabbitMqSendTopology :
        ISendTopology
    {
        IEntityNameValidator EntityNameValidator { get; }

        new IRabbitMqMessageSendTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class;

        /// <summary>
        /// Return the send settings for the specified <paramref name="address" />
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        SendSettings GetSendSettings(RabbitMqEndpointAddress address);

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
