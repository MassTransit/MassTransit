namespace MassTransit.Azure.ServiceBus.Core.Topology
{
    using MassTransit.Topology;
    using Transport;


    public interface IServiceBusSendTopology :
        ISendTopology
    {
        new IServiceBusMessageSendTopology<T> GetMessageTopology<T>()
            where T : class;

        SendSettings GetSendSettings(ServiceBusEndpointAddress address);

        SendSettings GetErrorSettings(IQueueConfigurator configurator);
        SendSettings GetDeadLetterSettings(IQueueConfigurator configurator);
    }
}
