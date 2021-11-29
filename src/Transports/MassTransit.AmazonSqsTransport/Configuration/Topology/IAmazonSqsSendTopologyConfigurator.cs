namespace MassTransit
{
    using System;
    using AmazonSqsTransport;
    using AmazonSqsTransport.Topology;
    using Topology;


    public interface IAmazonSqsSendTopologyConfigurator :
        ISendTopologyConfigurator,
        IAmazonSqsSendTopology
    {
        Action<IAmazonSqsQueueConfigurator> ConfigureErrorSettings { set; }
        Action<IAmazonSqsQueueConfigurator> ConfigureDeadLetterSettings { set; }
    }
}
