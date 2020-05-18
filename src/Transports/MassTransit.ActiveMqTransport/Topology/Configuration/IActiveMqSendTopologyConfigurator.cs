namespace MassTransit.ActiveMqTransport.Topology
{
    using System;
    using MassTransit.Topology;


    public interface IActiveMqSendTopologyConfigurator :
        ISendTopologyConfigurator,
        IActiveMqSendTopology
    {
        Action<IQueueConfigurator> ConfigureErrorSettings { set; }

        Action<IQueueConfigurator> ConfigureDeadLetterSettings { set; }
    }
}
