namespace MassTransit
{
    using SqlTransport;


    public interface ISqlSendTopology :
        ISendTopology
    {
        new ISqlMessageSendTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class;

        /// <summary>
        /// Return the send settings for the specified <paramref name="address" />
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        SendSettings GetSendSettings(SqlEndpointAddress address);

        /// <summary>
        /// Return the error settings for the queue
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        SendSettings GetErrorSettings(ReceiveSettings settings);

        /// <summary>
        /// Return the dead letter settings for the queue
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        SendSettings GetDeadLetterSettings(ReceiveSettings settings);
    }
}
