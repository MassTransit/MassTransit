namespace MassTransit
{
    using System;
    using ActiveMqTransport.Topology;


    public interface IActiveMqSendTopologyConfigurator :
        ISendTopologyConfigurator,
        IActiveMqSendTopology
    {
        Action<IActiveMqQueueConfigurator> ConfigureErrorSettings { set; }

        Action<IActiveMqQueueConfigurator> ConfigureDeadLetterSettings { set; }
    }
}
