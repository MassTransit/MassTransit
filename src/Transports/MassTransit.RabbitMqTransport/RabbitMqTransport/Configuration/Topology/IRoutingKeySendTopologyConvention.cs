namespace MassTransit.RabbitMqTransport.Configuration
{
    using MassTransit.Configuration;


    public interface IRoutingKeySendTopologyConvention :
        ISendTopologyConvention
    {
        /// <summary>
        /// The default, non-message specific routing key formatter used by messages
        /// when no specific convention has been specified.
        /// </summary>
        IRoutingKeyFormatter DefaultFormatter { get; set; }
    }
}
