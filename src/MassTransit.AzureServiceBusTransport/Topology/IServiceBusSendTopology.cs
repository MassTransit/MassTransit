namespace MassTransit.AzureServiceBusTransport.Topology
{
    using System;
    using Configuration;
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

        SendSettings GetErrorSettings(ISubscriptionConfigurator configurator, Uri hostAddress);
        SendSettings GetDeadLetterSettings(ISubscriptionConfigurator configurator, Uri hostAddress);
    }
}
