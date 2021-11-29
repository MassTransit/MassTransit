namespace MassTransit
{
    using ActiveMqTransport;
    using ActiveMqTransport.Topology;


    public interface IActiveMqSendTopology :
        ISendTopology
    {
        new IActiveMqMessageSendTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class;

        SendSettings GetSendSettings(ActiveMqEndpointAddress address);

        /// <summary>
        /// Return the error settings for the queue
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        ErrorSettings GetErrorSettings(EntitySettings settings);

        /// <summary>
        /// Return the dead letter settings for the queue
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        DeadLetterSettings GetDeadLetterSettings(EntitySettings settings);
    }
}
