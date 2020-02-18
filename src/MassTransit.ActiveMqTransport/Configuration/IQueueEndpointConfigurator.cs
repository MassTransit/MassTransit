namespace MassTransit.ActiveMqTransport
{
    public interface IQueueEndpointConfigurator :
        IQueueConfigurator
    {
        /// <summary>
        /// Specify the maximum number of concurrent messages that are consumed
        /// </summary>
        /// <value>The limit</value>
        ushort PrefetchCount { set; }
    }
}
