namespace MassTransit.Azure.ServiceBus.Core.Topology
{
    using System;
    using MassTransit.Topology;


    public interface IServiceBusSendTopologyConfigurator :
        ISendTopologyConfigurator,
        IServiceBusSendTopology
    {
        Action<IEntityConfigurator> ConfigureErrorSettings { set; }
        Action<IEntityConfigurator> ConfigureDeadLetterSettings { set; }

        new IServiceBusMessageSendTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class;
    }
}
