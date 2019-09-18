namespace MassTransit
{
    using System;
    using Azure.ServiceBus.Core;
    using Azure.ServiceBus.Core.Configuration;
    using Microsoft.Azure.WebJobs;
    using WebJobs.EventHubsIntegration;
    using WebJobs.EventHubsIntegration.Configuration;


    public static class AzureFunctionEventHubsBusExtensions
    {
        public static IEventDataReceiver CreateEventDataReceiver(this IBusFactorySelector selector, IBinder binder,
            Action<IWebJobReceiverConfigurator> configure)
        {
            if (binder == null)
                throw new ArgumentNullException(nameof(binder));

            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            var topologyConfiguration = new ServiceBusTopologyConfiguration(AzureBusFactory.MessageTopology);
            var busConfiguration = new ServiceBusBusConfiguration(topologyConfiguration);

            var endpointConfiguration = new BrokeredMessageReceiverServiceBusEndpointConfiguration(busConfiguration);

            var configurator = new WebJobEventDataReceiverSpecification(binder, endpointConfiguration);

            configure(configurator);

            return configurator.Build();
        }
    }
}
