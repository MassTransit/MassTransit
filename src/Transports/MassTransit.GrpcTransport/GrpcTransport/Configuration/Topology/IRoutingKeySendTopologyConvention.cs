namespace MassTransit.GrpcTransport.Configuration
{
    using MassTransit.Configuration;
    using Topology;


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
