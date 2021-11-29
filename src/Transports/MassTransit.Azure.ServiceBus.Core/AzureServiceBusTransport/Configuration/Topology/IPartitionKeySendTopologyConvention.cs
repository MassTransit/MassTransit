namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using MassTransit.Configuration;


    public interface IPartitionKeySendTopologyConvention :
        ISendTopologyConvention
    {
        /// <summary>
        /// The default, non-message specific routing key formatter used by messages
        /// when no specific convention has been specified.
        /// </summary>
        IPartitionKeyFormatter DefaultFormatter { get; set; }
    }
}
