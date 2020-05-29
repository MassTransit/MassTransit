namespace MassTransit.Azure.ServiceBus.Core.Topology.Conventions
{
    using MassTransit.Topology;


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
