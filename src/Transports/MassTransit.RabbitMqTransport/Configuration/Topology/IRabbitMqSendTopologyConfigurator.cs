namespace MassTransit
{
    using System;


    public interface IRabbitMqSendTopologyConfigurator :
        ISendTopologyConfigurator,
        IRabbitMqSendTopology
    {
        Action<IRabbitMqQueueBindingConfigurator> ConfigureErrorSettings { set; }
        Action<IRabbitMqQueueBindingConfigurator> ConfigureDeadLetterSettings { set; }
    }
}
