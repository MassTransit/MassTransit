namespace MassTransit.RabbitMqTransport.Topology
{
    using System;
    using MassTransit.Topology;


    public interface IRabbitMqSendTopologyConfigurator :
        ISendTopologyConfigurator,
        IRabbitMqSendTopology
    {
        Action<IQueueBindingConfigurator> ConfigureErrorSettings { set; }
        Action<IQueueBindingConfigurator> ConfigureDeadLetterSettings { set; }
    }
}
