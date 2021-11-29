namespace MassTransit
{
    using System;
    using RabbitMqTransport.Topology;


    public interface IRabbitMqSendTopologyConfigurator :
        ISendTopologyConfigurator,
        IRabbitMqSendTopology
    {
        Action<IRabbitMqQueueBindingConfigurator> ConfigureErrorSettings { set; }
        Action<IRabbitMqQueueBindingConfigurator> ConfigureDeadLetterSettings { set; }
    }
}
