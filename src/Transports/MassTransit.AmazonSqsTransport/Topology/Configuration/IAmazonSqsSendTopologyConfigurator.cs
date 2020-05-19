namespace MassTransit.AmazonSqsTransport.Topology
{
    using System;
    using MassTransit.Topology;


    public interface IAmazonSqsSendTopologyConfigurator :
        ISendTopologyConfigurator,
        IAmazonSqsSendTopology
    {
        Action<IQueueConfigurator> ConfigureErrorSettings { set; }
        Action<IQueueConfigurator> ConfigureDeadLetterSettings { set; }
    }
}
